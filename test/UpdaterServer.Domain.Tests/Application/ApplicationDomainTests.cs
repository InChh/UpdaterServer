using System.Threading.Tasks;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace UpdaterServer.Application;

public class ApplicationDomainTests<TStartupModule> : UpdaterServerDomainTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IRepository<Application> _applicationRepository;
    private readonly ApplicationManager _applicationManager;

    public ApplicationDomainTests()
    {
        _applicationRepository = GetRequiredService<IRepository<Application>>();
        _applicationManager = GetRequiredService<ApplicationManager>();
    }

    [Fact]
    public async Task Should_Create_Application()
    {
        var app1 = await _applicationManager.CreateAsync("App1", "Application 1");
        var app2 = await _applicationManager.CreateAsync("App2", "Application 2");
        await WithUnitOfWorkAsync(async () =>
        {
            await _applicationRepository.InsertAsync(app1);
            await _applicationRepository.InsertAsync(app2);
        });

        (await _applicationRepository.CountAsync()).ShouldBe(2);

        await _applicationManager.CreateAsync("App1", "Application 1")
            .ShouldThrowAsync<BusinessException>();

        var application = await _applicationRepository.GetAsync(a => a.Name == app1.Name);
        application.Name.ShouldBe("App1");
        application.Description.ShouldBe("Application 1");
    }

    [Fact]
    public async Task Should_Update_Application()
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
    }
}