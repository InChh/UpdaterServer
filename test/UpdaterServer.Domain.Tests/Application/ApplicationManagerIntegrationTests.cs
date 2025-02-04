using System.Threading.Tasks;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace UpdaterServer.Application;

public class ApplicationManagerIntegrationTests<TStartupModule> : UpdaterServerDomainTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IRepository<Application> _applicationRepository;
    private readonly ApplicationManager _applicationManager;

    public ApplicationManagerIntegrationTests()
    {
        _applicationRepository = GetRequiredService<IRepository<Application>>();
        _applicationManager = GetRequiredService<ApplicationManager>();
    }

    [Fact]
    public async Task Should_Throw_When_Application_Exists()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            await _applicationManager.CreateAsync("TestApp1", "foo")
                .ShouldThrowAsync<BusinessException>();
        });

        var application = await _applicationRepository.GetAsync(a => a.Name == "TestApp1");
        application.Name.ShouldBe("TestApp1");
        application.Description.ShouldBe("Test Application 1");
    }

    [Fact]
    public async Task Should_Update_Application()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var app = await _applicationRepository.FindAsync(a => a.Name == "TestApp1");
            app.ShouldNotBeNull();
            await _applicationManager.UpdateAsync(app, "App1", "Application 1");
            await _applicationRepository.UpdateAsync(app, true);

            var application = await _applicationRepository.GetAsync(a => a.Id == app.Id);

            application.Name.ShouldBe("App1");
            application.Description.ShouldBe("Application 1");
            await _applicationManager.UpdateAsync(application, "TestApp2", "Test Application 2")
                .ShouldThrowAsync<BusinessException>();
        });
    }
}