using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using UpdaterServer.Application;
using UpdaterServer.File;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
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
            MaxResultCount = UpdaterServerTestConsts.TestApplicationCount
        });

        return apps.Items[new Random().Next(0, apps.Items.Count)];
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

    [Fact]
    public async Task UpdateAsync_Should_Work()
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

        input.VersionNumber = $"{year}.{month}.{UpdaterServerTestConsts.TestApplicationVersionCount + 2}";
        input.Description = "Test Version 2";

        version = await _applicationVersionAppService.UpdateAsync(version.Id, input);
        version.ShouldNotBeNull();
        version.VersionNumber.ShouldBe(input.VersionNumber);
        version.Description.ShouldBe(input.Description);
        version.IsActive.ShouldBeFalse();

        var files = await _applicationVersionAppService.GetFilesByIdAsync(version.Id);
        files.ShouldNotBeNull();
        files.Files.Count.ShouldBe(input.FileMetadataIds.Count);

        var newFiles = await _fileAppService.GetListAsync(new GetFilesRequestDto()
        {
            SkipCount = 10,
            MaxResultCount = 10
        });

        var newFileMetadataIds = newFiles.Items.Select(f => f.Id).ToList();
        input.FileMetadataIds.AddRange(newFileMetadataIds);

        version = await _applicationVersionAppService.UpdateAsync(version.Id, input);
        version.ShouldNotBeNull();
        version.VersionNumber.ShouldBe(input.VersionNumber);
        version.Description.ShouldBe(input.Description);
        version.IsActive.ShouldBeFalse();

        files = await _applicationVersionAppService.GetFilesByIdAsync(version.Id);
        files.ShouldNotBeNull();
        files.Files.Count.ShouldBe(input.FileMetadataIds.Count);
    }

    [Fact]
    public async Task UpdateAsync_Should_Throw_EntityNotFoundException_When_VersionId_Not_Exist()
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

        var exception = await _applicationVersionAppService.UpdateAsync(Guid.NewGuid(), input)
            .ShouldThrowAsync<EntityNotFoundException>();

        exception.EntityType.ShouldBe(typeof(ApplicationVersion));
    }

    [Fact]
    public async Task UpdateAsync_Should_Throw_BussinessException_When_VersionNumber_Not_Valid()
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

        // Version number already exists
        input.VersionNumber = $"{year}.{month}.{UpdaterServerTestConsts.TestApplicationVersionCount}";

        var exception = await _applicationVersionAppService.UpdateAsync(version.Id, input)
            .ShouldThrowAsync<BusinessException>();

        exception.Code.ShouldBe(ApplicationVersionErrorCodes.VersionNumberAlreadyExist);
        exception.Data["versionNumber"].ShouldBe(input.VersionNumber);

        // version number is invalid
        input.VersionNumber = "1234";

        exception = await _applicationVersionAppService.UpdateAsync(version.Id, input)
            .ShouldThrowAsync<BusinessException>();

        exception.Code.ShouldBe(ApplicationVersionErrorCodes.VersionNumberInvalid);
        exception.Data["versionNumber"].ShouldBe(input.VersionNumber);

        input.VersionNumber = "2000.1.1";

        exception = await _applicationVersionAppService.UpdateAsync(version.Id, input)
            .ShouldThrowAsync<BusinessException>();

        exception.Code.ShouldBe(ApplicationVersionErrorCodes.VersionNumberInvalid);
        exception.Data["versionNumber"].ShouldBe(input.VersionNumber);

        input.VersionNumber = $"{year}.{month}.0";

        exception = await _applicationVersionAppService.UpdateAsync(version.Id, input)
            .ShouldThrowAsync<BusinessException>();

        exception.Code.ShouldBe(ApplicationVersionErrorCodes.VersionNumberInvalid);
        exception.Data["versionNumber"].ShouldBe(input.VersionNumber);

        input.VersionNumber = $"{year}.{month}.-1";

        exception = await _applicationVersionAppService.UpdateAsync(version.Id, input)
            .ShouldThrowAsync<BusinessException>();

        exception.Code.ShouldBe(ApplicationVersionErrorCodes.VersionNumberInvalid);
        exception.Data["versionNumber"].ShouldBe(input.VersionNumber);
    }

    [Fact]
    public async Task UpdateAsync_Should_Throw_BussinessException_When_FileMetadataIds_Not_Exist()
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

        var notExistingFile = Guid.NewGuid();
        fileMetadataIds.Add(notExistingFile);

        input.FileMetadataIds = fileMetadataIds;

        var exception = await _applicationVersionAppService.UpdateAsync(version.Id, input)
            .ShouldThrowAsync<BusinessException>();

        exception.Code.ShouldBe(ApplicationVersionErrorCodes.FileDoesNotExist);
        exception.Data["missingFiles"].ShouldBe(new[] { notExistingFile });
    }

    [Fact]
    public async Task ActiveAsync_DeactiveAsync_Should_Work()
    {
        var application = await GetApplicationAsync();

        var year = DateTime.Now.Year;
        var month = DateTime.Now.Month;

        var fileMetadataIds = await GetFileMetadataIdsAsync();

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

        await _applicationVersionAppService.ActiveAsync(version.Id);

        version = await _applicationVersionAppService.GetAsync(version.Id);

        version.ShouldNotBeNull();
        version.IsActive.ShouldBeTrue();

        await _applicationVersionAppService.DeactiveAsync(version.Id);

        version = await _applicationVersionAppService.GetAsync(version.Id);

        version.ShouldNotBeNull();
        version.IsActive.ShouldBeFalse();
    }

    [Fact]
    public async Task GetLatestAsync_Should_Work()
    {
        var application = await GetApplicationAsync();

        var year = DateTime.Now.Year;
        var month = DateTime.Now.Month;

        var latest = await _applicationVersionAppService.GetLatestAsync(application.Id);

        latest.ShouldNotBeNull();
        latest.IsActive.ShouldBeTrue();
        latest.VersionNumber.ShouldBe($"{year}.{month}.{UpdaterServerTestConsts.TestApplicationVersionCount}");

        var fileMetadataIds = await GetFileMetadataIdsAsync();
        var input = new CreateUpdateApplicationVersionDto
        {
            ApplicationId = application.Id,
            VersionNumber = $"{year}.{month}.{UpdaterServerTestConsts.TestApplicationVersionCount + 1}",
            Description = "New Test Version",
            FileMetadataIds = fileMetadataIds
        };

        var version = await _applicationVersionAppService.CreateAsync(input);

        version.ShouldNotBeNull();
        version.VersionNumber.ShouldBe(input.VersionNumber);
        version.Description.ShouldBe(input.Description);
        version.IsActive.ShouldBeFalse();
        await _applicationVersionAppService.ActiveAsync(version.Id);

        latest = await _applicationVersionAppService.GetLatestAsync(application.Id);

        latest.ShouldNotBeNull();
        latest.VersionNumber.ShouldBe(input.VersionNumber);
        latest.Description.ShouldBe(input.Description);
        latest.IsActive.ShouldBeTrue();

        await _applicationVersionAppService.DeactiveAsync(version.Id);

        latest = await _applicationVersionAppService.GetLatestAsync(application.Id);

        latest.ShouldNotBeNull();
        latest.VersionNumber.ShouldBe($"{year}.{month}.{UpdaterServerTestConsts.TestApplicationVersionCount}");
        latest.IsActive.ShouldBeTrue();

        await _applicationVersionAppService.GetLatestAsync(Guid.NewGuid()).ShouldThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task GetListAsync_Should_Work()
    {
        var application = await GetApplicationAsync();

        var input = new PagedAndSortedResultRequestDto()
        {
            MaxResultCount = UpdaterServerTestConsts.TestApplicationVersionCount
        };

        var versions = await _applicationVersionAppService.GetListAsync(application.Id, input);

        versions.ShouldNotBeNull();
        versions.TotalCount.ShouldBe(UpdaterServerTestConsts.TestApplicationVersionCount);
        versions.Items.Count.ShouldBe(input.MaxResultCount);

        input = new PagedAndSortedResultRequestDto()
        {
            SkipCount = UpdaterServerTestConsts.TestApplicationVersionCount / 2,
            MaxResultCount = UpdaterServerTestConsts.TestApplicationVersionCount -
                             UpdaterServerTestConsts.TestApplicationVersionCount / 2,
        };

        versions = await _applicationVersionAppService.GetListAsync(application.Id, input);

        versions.ShouldNotBeNull();
        versions.TotalCount.ShouldBe(UpdaterServerTestConsts.TestApplicationVersionCount);
        versions.Items.Count.ShouldBe(input.MaxResultCount);
    }

    [Fact]
    public async Task GetFilesByVersionNumberAsync_Should_Work()
    {
        var application = await GetApplicationAsync();

        var version = await _applicationVersionAppService.GetLatestAsync(application.Id);

        var files = await _applicationVersionAppService.GetFilesByVersionNumberAsync(application.Id,
            version.VersionNumber);

        files.ShouldNotBeNull();
        files.Files.Count.ShouldBe(UpdaterServerTestConsts.TestFileMetadataCount);

        await _applicationVersionAppService.GetFilesByVersionNumberAsync(Guid.NewGuid(), version.VersionNumber)
            .ShouldThrowAsync<EntityNotFoundException>();

        await _applicationVersionAppService.GetFilesByVersionNumberAsync(application.Id, "1.1.1")
            .ShouldThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task GetFilesByIdAsync_Should_Work()
    {
        var application = await GetApplicationAsync();

        var year = DateTime.Now.Year;
        var month = DateTime.Now.Month;

        var fileMetadataIds = await GetFileMetadataIdsAsync();

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

        var files = await _applicationVersionAppService.GetFilesByIdAsync(version.Id);

        files.ShouldNotBeNull();
        files.Files.Count.ShouldBe(input.FileMetadataIds.Count);

        await _applicationVersionAppService.GetFilesByIdAsync(Guid.NewGuid())
            .ShouldThrowAsync<EntityNotFoundException>();
    }
    
    [Fact]
    public async Task DeleteAsync_Should_Work()
    {
        var application = await GetApplicationAsync();

        var year = DateTime.Now.Year;
        var month = DateTime.Now.Month;

        var fileMetadataIds = await GetFileMetadataIdsAsync();

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

        await _applicationVersionAppService.DeleteAsync(version.Id);

        await _applicationVersionAppService.GetAsync(version.Id).ShouldThrowAsync<EntityNotFoundException>();
    }
}