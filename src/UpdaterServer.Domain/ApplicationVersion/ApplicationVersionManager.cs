using System;
using System.Linq;
using System.Threading.Tasks;
using UpdaterServer.File;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Guids;
using Volo.Abp.Linq;

namespace UpdaterServer.ApplicationVersion;

public class ApplicationVersionManager(
    IGuidGenerator guidGenerator,
    IRepository<Application.Application> applicationRepository,
    IRepository<ApplicationVersion> versionRepository,
    IRepository<FileMetadata> fileRepository,
    IAsyncQueryableExecuter queryableExecutor) : DomainService
{
    public async Task<ApplicationVersion> CreateAsync(
        Application.Application application,
        string versionNumber,
        string description,
        FileMetadata[] files)
    {
        // Check if the application exists.
        var c = await applicationRepository.CountAsync(a => a.Id == application.Id);
        if (c == 0)
        {
            throw new BusinessException(ApplicationVersionErrorCodes.ApplicationDoesNotExist).WithData("name",
                application.Name);
        }

        await CheckVersionNumber(application.Id, versionNumber);

        var version = new ApplicationVersion(guidGenerator.Create(), application.Id, versionNumber, description);

        await SetFilesAsync(version, files);

        return version;
    }

    public async Task UpdateAsync(
        ApplicationVersion version,
        string? versionNumber,
        string? description,
        FileMetadata[]? files)
    {
        var application = await applicationRepository.GetAsync(a => a.Id == version.ApplicationId);

        if (versionNumber is not null)
        {
            await CheckVersionNumber(application.Id, versionNumber);
            version.VersionNumber = versionNumber;
        }

        if (description is not null)
        {
            version.Description = description;
        }

        if (files is not null)
        {
            await SetFilesAsync(version, files);
        }
    }

    private async Task CheckVersionNumber(
        Guid applicationId,
        string versionNumber)
    {
        // Check if the version number exists.
        var c = await versionRepository.CountAsync(v =>
            v.ApplicationId == applicationId && v.VersionNumber == versionNumber);
        if (c != 0)
        {
            throw new BusinessException(ApplicationVersionErrorCodes.VersionNumberAlreadyExist)
                .WithData("versionNumber", versionNumber);
        }

        // Check if the version number is the newest.
        var queryable = await versionRepository.GetQueryableAsync();
        queryable = queryable.Where(v => v.ApplicationId == applicationId);
        var versions = await queryableExecutor.ToListAsync(queryable);
        var newest = versions
            .OrderBy(a => a.VersionNumber, new ApplicationVersionComparer())
            .LastOrDefault();


        if (newest is null)
        {
            return;
        }

        if (new ApplicationVersionComparer().Compare(versionNumber, newest.VersionNumber) <= 0)
        {
            throw new BusinessException(ApplicationVersionErrorCodes.VersionNumberShouldBeNewer)
                .WithData("versionNumber", versionNumber)
                .WithData("newestVersionNumber", newest.VersionNumber);
        }
    }

    private async Task SetFilesAsync(ApplicationVersion version, FileMetadata[] files)
    {
        // If there are no files, remove all files.
        if (files.Length == 0)
        {
            version.RemoveAllFiles();
            return;
        }

        // Find fileMetadataIds from database.
        var query = (await fileRepository.GetQueryableAsync())
            .Where(x => files.Select(f => new { f.Hash, f.Size }).Contains(new { x.Hash, x.Size }))
            .Select(x => x.Id);

        var fileIds = await queryableExecutor.ToListAsync(query);

        // If not all fileMetadata are found from database, get missing file names and throw an exception.
        if (fileIds.Count != files.Length)
        {
            // get the missing files
            var missingFiles = files.Where(f => !fileIds.Contains(f.Id))
                .Select(f => f.Path)
                .ToArray();

            throw new BusinessException(ApplicationVersionErrorCodes.FileDoesNotExist)
                .WithData("missingFiles", missingFiles);
        }

        version.RemoveAllFilesExceptGivenIds(fileIds);
        foreach (var fileId in fileIds)
        {
            version.AddFile(fileId);
        }
    }
}