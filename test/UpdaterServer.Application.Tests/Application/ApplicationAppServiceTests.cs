using System;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Xunit;

namespace UpdaterServer.Application;

public class ApplicationAppServiceTests<TStartupModule> : UpdaterServerApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IApplicationAppService _applicationAppService;

    public ApplicationAppServiceTests()
    {
        _applicationAppService = GetRequiredService<IApplicationAppService>();
    }

    [Fact]
    public async Task CreateAsync_Should_Work()
    {
        var input = new CreateUpdateApplicationDto
        {
            Name = "AnotherTestApp",
            Description = "Another TestApp Description"
        };

        var app = await _applicationAppService.CreateAsync(input);

        app.ShouldNotBeNull();
        app.Name.ShouldBe(input.Name);
        app.Description.ShouldBe(input.Description);

        input = new CreateUpdateApplicationDto()
        {
            Name = "AnotherTestApp2",
        };

        app = await _applicationAppService.CreateAsync(input);

        app.ShouldNotBeNull();
        app.Name.ShouldBe(input.Name);
        app.Description.ShouldBeNull();
    }

    [Fact]
    public async Task CreateAsync_Should_Throw_BusinessException_When_Application_Already_Exists()
    {
        var input = new CreateUpdateApplicationDto
        {
            Name = "TestApp1",
            Description = "TestApp Description 1"
        };

        await _applicationAppService.CreateAsync(input).ShouldThrowAsync<BusinessException>();
    }

    [Fact]
    public async Task CreateAsync_Should_Throw_ValidationsException_When_Input_Is_Invalid()
    {
        var input = new CreateUpdateApplicationDto
        {
            Name = "",
            Description = "TestApp Description"
        };

        await _applicationAppService.CreateAsync(input).ShouldThrowAsync<AbpValidationException>();

        input = new CreateUpdateApplicationDto
        {
            Name = "AnotherTestApp",
            Description = ""
        };

        await _applicationAppService.CreateAsync(input).ShouldNotThrowAsync();

        input = new CreateUpdateApplicationDto()
        {
            Name = "AnotherTestApp1",
        };

        await _applicationAppService.CreateAsync(input).ShouldNotThrowAsync();

        input = new CreateUpdateApplicationDto()
        {
            Name = new string('a', 129),
        };

        await _applicationAppService.CreateAsync(input).ShouldThrowAsync<AbpValidationException>();

        input = new CreateUpdateApplicationDto()
        {
            Name = "AnotherTestApp2",
            Description = new string('a', 600),
        };

        await _applicationAppService.CreateAsync(input).ShouldThrowAsync<AbpValidationException>();
    }

    [Fact]
    public async Task UpdateAsync_Should_Work()
    {
        var input = new CreateUpdateApplicationDto
        {
            Name = "AnotherTestApp",
            Description = "Another TestApp Description"
        };

        var app = await _applicationAppService.CreateAsync(input);

        app.ShouldNotBeNull();
        app.Name.ShouldBe(input.Name);
        app.Description.ShouldBe(input.Description);

        var updateInput = new CreateUpdateApplicationDto
        {
            Name = "AnotherTestAppUpdated",
            Description = "Another TestApp Description Updated"
        };

        app = await _applicationAppService.UpdateAsync(app.Id, updateInput);

        app.ShouldNotBeNull();
        app.Name.ShouldBe(updateInput.Name);
        app.Description.ShouldBe(updateInput.Description);
    }

    [Fact]
    public async Task UpdateAsync_Should_Throw_EntityNotFoundException_When_Application_Not_Found()
    {
        var input = new CreateUpdateApplicationDto
        {
            Name = "TestApp1",
            Description = "TestApp Description 1"
        };

        await _applicationAppService.UpdateAsync(Guid.NewGuid(), input).ShouldThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task UpdateAsync_Should_Throw_BusinessException_When_Application_Name_Already_Exists()
    {
        var input = new CreateUpdateApplicationDto
        {
            Name = "App",
            Description = "Description"
        };

        var app = await _applicationAppService.CreateAsync(input);

        app.ShouldNotBeNull();
        app.Name.ShouldBe(input.Name);
        app.Description.ShouldBe(input.Description);

        var updateInput = new CreateUpdateApplicationDto
        {
            Name = "TestApp1",
            Description = "TestApp Description 1"
        };

        await _applicationAppService.UpdateAsync(app.Id, updateInput).ShouldThrowAsync<BusinessException>();
    }

    [Fact]
    public async Task GetAsync_Should_Work()
    {
        var input = new CreateUpdateApplicationDto
        {
            Name = "AnotherTestApp",
            Description = "Another TestApp Description"
        };

        var app = await _applicationAppService.CreateAsync(input);

        app.ShouldNotBeNull();
        app.Name.ShouldBe(input.Name);
        app.Description.ShouldBe(input.Description);

        var app2 = await _applicationAppService.GetAsync(app.Id);

        app2.ShouldNotBeNull();
        app2.Name.ShouldBe(app.Name);
        app2.Description.ShouldBe(app.Description);
    }

    [Fact]
    public async Task GetAsync_Should_Throw_EntityNotFoundException_When_Application_Not_Found()
    {
        await _applicationAppService.GetAsync(Guid.NewGuid()).ShouldThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task GetListAsync_Paginate_Should_Work()
    {
        var input = new GetApplicationListDto()
        {
            MaxResultCount = 999,
            SkipCount = 0
        };
        var apps = await _applicationAppService.GetListAsync(input);

        apps.ShouldNotBeNull();
        apps.TotalCount.ShouldBe(UpdaterServerTestConsts.TestApplicationCount);
        apps.Items.Count.ShouldBe(UpdaterServerTestConsts.TestApplicationCount);

        input = new GetApplicationListDto()
        {
            MaxResultCount = 2,
            SkipCount = 0
        };

        apps = await _applicationAppService.GetListAsync(input);

        apps.ShouldNotBeNull();
        apps.TotalCount.ShouldBe(UpdaterServerTestConsts.TestApplicationCount);
        apps.Items.Count.ShouldBe(2);

        input = new GetApplicationListDto()
        {
            MaxResultCount = 2,
            SkipCount = 2
        };

        apps = await _applicationAppService.GetListAsync(input);

        apps.ShouldNotBeNull();
        apps.TotalCount.ShouldBe(UpdaterServerTestConsts.TestApplicationCount);
        apps.Items.Count.ShouldBe(2);

        input = new GetApplicationListDto()
        {
            MaxResultCount = 4,
            SkipCount = 3
        };

        apps = await _applicationAppService.GetListAsync(input);

        apps.ShouldNotBeNull();
        apps.TotalCount.ShouldBe(UpdaterServerTestConsts.TestApplicationCount);
        apps.Items.Count.ShouldBe(1);
    }

    [Fact]
    public async Task GetListAsync_Filter_Should_Work()
    {
        var input = new GetApplicationListDto()
        {
            MaxResultCount = 999,
            SkipCount = 0,
            Filter = "TestApp1"
        };
        var apps = await _applicationAppService.GetListAsync(input);

        apps.ShouldNotBeNull();
        apps.TotalCount.ShouldBe(1);
        apps.Items.Count.ShouldBe(1);
        apps.Items[0].Name.ShouldBe("TestApp1");

        input = new GetApplicationListDto()
        {
            MaxResultCount = 999,
            SkipCount = 0,
            Filter = "TestApp"
        };
        apps = await _applicationAppService.GetListAsync(input);

        apps.ShouldNotBeNull();
        apps.TotalCount.ShouldBe(4);
        apps.Items.Count.ShouldBe(4);
    }

    [Fact]
    public async Task GetListAsync_Sorting_Should_Work()
    {
        var input = new GetApplicationListDto()
        {
            MaxResultCount = 999,
            SkipCount = 0,
            Sorting = "name"
        };
        var apps = await _applicationAppService.GetListAsync(input);

        apps.ShouldNotBeNull();
        apps.TotalCount.ShouldBe(UpdaterServerTestConsts.TestApplicationCount);
        apps.Items.Count.ShouldBe(UpdaterServerTestConsts.TestApplicationCount);
        apps.Items[0].Name.ShouldBe("TestApp1");

        input = new GetApplicationListDto()
        {
            MaxResultCount = 999,
            SkipCount = 0,
            Sorting = "name desc"
        };

        apps = await _applicationAppService.GetListAsync(input);

        apps.ShouldNotBeNull();
        apps.TotalCount.ShouldBe(UpdaterServerTestConsts.TestApplicationCount);
        apps.Items.Count.ShouldBe(UpdaterServerTestConsts.TestApplicationCount);
        apps.Items[^1].Name.ShouldBe("TestApp1");
    }
}