using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using UpdaterServer.File;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace UpdaterServer.ApplicationVersion;

public class ApplicationVersionAppService(
    ApplicationVersionManager applicationVersionManager,
    IRepository<ApplicationVersion> versionRepository,
    IRepository<Application.Application> applicationRepository,
    IRepository<FileMetadata> fileMetadataRepository)
    : ApplicationService, IApplicationVersionAppService
{
    [Authorize]
    public async Task<ApplicationVersionDto> CreateAsync(CreateUpdateApplicationVersionDto input)
    {
        var application = await applicationRepository.GetAsync(a => a.Id == input.ApplicationId);
        var version = await applicationVersionManager.CreateAsync(application, input.VersionNumber, input.Description,
            input.FileMetadataIds);
        await versionRepository.InsertAsync(version);
        return ObjectMapper.Map<ApplicationVersion, ApplicationVersionDto>(version);
    }

    [Authorize]
    public async Task<ApplicationVersionDto> UpdateAsync(Guid id, CreateUpdateApplicationVersionDto input)
    {
        var version = await versionRepository.GetAsync(v => v.Id == id);
        await applicationVersionManager.UpdateAsync(version, input.VersionNumber, input.Description,
            input.FileMetadataIds);
        return ObjectMapper.Map<ApplicationVersion, ApplicationVersionDto>(version);
    }

    public async Task<ApplicationVersionDto> GetLatestAsync(Guid applicationId)
    {
        var versions = await versionRepository.GetListAsync();

        var latest = versions
            .OrderBy(v => v.VersionNumber, new ApplicationVersionComparer())
            .LastOrDefault();

        if (latest == null)
        {
            throw new BusinessException(ApplicationVersionErrorCodes.VersionDoesNotExist).WithData("applicationId",
                applicationId);
        }

        return ObjectMapper.Map<ApplicationVersion, ApplicationVersionDto>(latest);
    }

    [Authorize]
    public async Task<ApplicationVersionDto> GetAsync(Guid id)
    {
        var version = await versionRepository.GetAsync(v => v.Id == id);
        return ObjectMapper.Map<ApplicationVersion, ApplicationVersionDto>(version);
    }

    [Authorize]
    public async Task<PagedResultDto<ApplicationVersionDto>> GetListAsync(Guid applicationId,
        PagedAndSortedResultRequestDto input)
    {
        var query = (await versionRepository.GetQueryableAsync())
            .Where(v => v.ApplicationId == applicationId);

        var total = await AsyncExecuter.CountAsync(query);
        query = query.OrderBy(input.Sorting ?? nameof(ApplicationVersion.CreationTime))
            .PageBy(input.SkipCount, input.MaxResultCount);

        var items = await AsyncExecuter.ToListAsync(query);

        return new PagedResultDto<ApplicationVersionDto>
        {
            TotalCount = total,
            Items = ObjectMapper.Map<List<ApplicationVersion>, List<ApplicationVersionDto>>(items)
        };
    }

    public async Task<ApplicationVersionFilesDto> GetFilesByVersionNumberAsync(Guid applicationId, string versionNumber)
    {
        var version = await versionRepository.GetAsync(v =>
            v.ApplicationId == applicationId && v.VersionNumber == versionNumber);

        var fileIds = version.Files.Select(f => f.FileMetadataId).ToArray();

        var files = await fileMetadataRepository.GetListAsync(f => fileIds.Contains(f.Id));

        return new ApplicationVersionFilesDto
        {
            Files = ObjectMapper.Map<List<FileMetadata>, List<FileMetadataDto>>(files)
        };
    }

    public async Task<ApplicationVersionFilesDto> GetFilesByIdAsync(Guid id)
    {
        var version = await versionRepository.GetAsync(v => v.Id == id);

        var fileIds = version.Files.Select(f => f.FileMetadataId).ToArray();

        var files = await fileMetadataRepository.GetListAsync(f => fileIds.Contains(f.Id));

        return new ApplicationVersionFilesDto
        {
            Files = ObjectMapper.Map<List<FileMetadata>, List<FileMetadataDto>>(files)
        };
    }

    [Authorize]
    public async Task<ApplicationVersionDto> DeleteAsync(Guid id)
    {
        var version = await versionRepository.GetAsync(v => v.Id == id);
        await versionRepository.DeleteAsync(version);
        return ObjectMapper.Map<ApplicationVersion, ApplicationVersionDto>(version);
    }

    [Authorize]
    public async Task ActiveAsync(Guid id)
    {
        var version = await versionRepository.GetAsync(v => v.Id == id);
        version.Active();
        await versionRepository.UpdateAsync(version);
    }

    [Authorize]
    public async Task DeactiveAsync(Guid id)
    {
        var version = await versionRepository.GetAsync(v => v.Id == id);
        version.Deactive();
        await versionRepository.UpdateAsync(version);
    }
}