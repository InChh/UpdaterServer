using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using UpdaterServer.Application;
using UpdaterServer.File;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Modularity;
using Xunit;

namespace UpdaterServer.ApplicationVersion;

public class ApplicationVersionAppServiceTests<TStartupModule> : UpdaterServerApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IApplicationVersionAppService _applicationVersionAppService;
    private readonly IApplicationAppService _applicationAppService;
    private readonly IFileAppService _fileAppService;

    public ApplicationVersionAppServiceTests()
    {
        _applicationVersionAppService = GetRequiredService<IApplicationVersionAppService>();
        _applicationAppService = GetRequiredService<IApplicationAppService>();
        _fileAppService = GetRequiredService<IFileAppService>();
    }

    private async Task<ApplicationDto> GetApplicationAsync()
    {
        var apps = await _applicationAppService.GetListAsync(new GetApplicationListDto()
        {
            MaxResultCount = 1
        });
        return apps.Items[0];
    }

    private async Task<List<Guid>> GetFileMetadataIdsAsync()
    {
        var files = await _fileAppService.GetListAsync(new GetFilesRequestDto()
        {
            MaxResultCount = 10
        });
        return files.Items.Select(f => f.Id).ToList();
    }

    [Fact]
    public async Task CreateAsync_Should_Work()
    {
        var application = await GetApplicationAsync();

        var fileMetadataIds = await GetFileMetadataIdsAsync();

        var year = DateTime.Now.Year;
        var month = DateTime.Now.Month;
        var input = new CreateUpdateApplicationVersionDto
        {
            ApplicationId = application.Id,
            VersionNumber = $"{year}.{month}.{UpdaterServerTestConsts.TestApplicationVersionCount + 1}",
            Description = "Test Version",
            FileMetadataIds = fileMetadataIds
        };

        var version = await _applicationVersionAppService.CreateAsync(input);

        version.ShouldNotBeNull();
        version.VersionNumber.ShouldBe(input.VersionNumber);
        version.Description.ShouldBe(input.Description);
        version.IsActive.ShouldBeFalse();

        input = new CreateUpdateApplicationVersionDto()
        {
            ApplicationId = application.Id,
            VersionNumber = $"{year}.{month}.{UpdaterServerTestConsts.TestApplicationVersionCount + 2}",
            Description = "Test Version",
            FileMetadataIds = fileMetadataIds
        };

        version = await _applicationVersionAppService.CreateAsync(input);

        version.ShouldNotBeNull();
        version.VersionNumber.ShouldBe(input.VersionNumber);
        version.Description.ShouldBe(input.Description);
        version.IsActive.ShouldBeFalse();
    }

    [Fact]
    public async Task CreateAsync_Should_Throw_EntityNotFoundException_When_ApplicationId_Not_Exist()
    {
        var fileMetadataIds = await GetFileMetadataIdsAsync();

        var year = DateTime.Now.Year;
        var month = DateTime.Now.Month;
        var input = new CreateUpdateApplicationVersionDto
        {
            ApplicationId = Guid.NewGuid(),
            VersionNumber = $"{year}.{month}.{UpdaterServerTestConsts.TestApplicationVersionCount + 1}",
            Description = "Test Version",
            FileMetadataIds = fileMetadataIds
        };

        var exception = await _applicationVersionAppService.CreateAsync(input)
            .ShouldThrowAsync<EntityNotFoundException>();

        exception.EntityType.ShouldBe(typeof(Application.Application));
    }

    [Fact]
    public async Task CreateAsync_Should_Throw_BussinessException_When_VersionNumber_Not_Valid()
    {
        var application = await GetApplicationAsync();

        var fileMetadataIds = await GetFileMetadataIdsAsync();

        var year = DateTime.Now.Year;
        var month = DateTime.Now.Month;
        // Version number already exists
        var input = new CreateUpdateApplicationVersionDto
        {
            ApplicationId = application.Id,
            VersionNumber = $"{year}.{month}.{UpdaterServerTestConsts.TestApplicationVersionCount}",
            Description = "Test Version",
            FileMetadataIds = fileMetadataIds
        };

        var exception = await _applicationVersionAppService.CreateAsync(input)
            .ShouldThrowAsync<BusinessException>();

        exception.Code.ShouldBe(ApplicationVersionErrorCodes.VersionNumberAlreadyExist);
        exception.Data["versionNumber"].ShouldBe(input.VersionNumber);

        // version number is invalid
        input.VersionNumber = "1234";

        exception = await _applicationVersionAppService.CreateAsync(input)
            .ShouldThrowAsync<BusinessException>();

        exception.Code.ShouldBe(ApplicationVersionErrorCodes.VersionNumberInvalid);
        exception.Data["versionNumber"].ShouldBe(input.VersionNumber);

        input.VersionNumber = "2000.1.1";

        exception = await _applicationVersionAppService.CreateAsync(input)
            .ShouldThrowAsync<BusinessException>();

        exception.Code.ShouldBe(ApplicationVersionErrorCodes.VersionNumberInvalid);
        exception.Data["versionNumber"].ShouldBe(input.VersionNumber);

        input.VersionNumber = $"{year}.{month}.0";

        exception = await _applicationVersionAppService.CreateAsync(input)
            .ShouldThrowAsync<BusinessException>();

        exception.Code.ShouldBe(ApplicationVersionErrorCodes.VersionNumberInvalid);
        exception.Data["versionNumber"].ShouldBe(input.VersionNumber);

        input.VersionNumber = $"{year}.{month}.-1";

        exception = await _applicationVersionAppService.CreateAsync(input)
            .ShouldThrowAsync<BusinessException>();

        exception.Code.ShouldBe(ApplicationVersionErrorCodes.VersionNumberInvalid);
        exception.Data["versionNumber"].ShouldBe(input.VersionNumber);
    }

    [Fact]
    public async Task CreateAsync_Should_Throw_BussinessException_When_FileMetadataIds_Not_Exist()
    {
        var application = await GetApplicationAsync();

        var fileMetadataIds = await GetFileMetadataIdsAsync();
        var notExistingFile = Guid.NewGuid();
        fileMetadataIds.Add(notExistingFile);

        var year = DateTime.Now.Year;
        var month = DateTime.Now.Month;
        var input = new CreateUpdateApplicationVersionDto
        {
            ApplicationId = application.Id,
            VersionNumber = $"{year}.{month}.{UpdaterServerTestConsts.TestApplicationVersionCount + 1}",
            Description = "Test Version",
            FileMetadataIds = fileMetadataIds
        };

        var exception = await _applicationVersionAppService.CreateAsync(input)
            .ShouldThrowAsync<BusinessException>();

        exception.Code.ShouldBe(ApplicationVersionErrorCodes.FileDoesNotExist);
        exception.Data["missingFiles"].ShouldBe(new[] { notExistingFile });
    }
}