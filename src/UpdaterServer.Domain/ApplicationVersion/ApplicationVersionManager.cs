using System;
using System.Linq;
using System.Threading.Tasks;
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
    IAsyncQueryableExecuter queryableExecuter) : DomainService
{
    public async Task<ApplicationVersion> CreateAsync(Application.Application application, string versionNumber,
        string description)
    {
        // Check if the application exists.
        var c = await applicationRepository.CountAsync(a => a.Id == application.Id);
        if (c == 0)
        {
            throw new BusinessException(ApplicationVersionErrorCodes.ApplicationDoesNotExist).WithData("name",
                application.Name);
        }

        // Check if the version number exists.
        c = await versionRepository.CountAsync(v =>
            v.ApplicationId == application.Id && v.VersionNumber == versionNumber);
        if (c != 0)
        {
            throw new BusinessException(ApplicationVersionErrorCodes.VersionNumberAlreadyExist)
                .WithData("versionNumber", versionNumber)
                .WithData("applicationName", application.Name);
        }

        // Check if the version number is the newest.
        var queryable = await versionRepository.GetQueryableAsync();
        queryable = queryable.Where(v => v.ApplicationId == application.Id)
            .OrderByDescending(v => v.VersionNumber);
        var newest = await queryableExecuter.FirstOrDefaultAsync(queryable);
        if (newest is null)
        {
            return new ApplicationVersion(guidGenerator.Create(), application.Id, versionNumber, description);
        }

        if (string.CompareOrdinal(newest.VersionNumber, versionNumber) > 0)
        {
            throw new BusinessException(ApplicationVersionErrorCodes.VersionNumberShouldBeNewer)
                .WithData("versionNumber", versionNumber)
                .WithData("newestVersionNumber", newest.VersionNumber);
        }

        return new ApplicationVersion(guidGenerator.Create(), application.Id, versionNumber, description);
    }
}