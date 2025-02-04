using System;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;
using Volo.Abp.Modularity;
using Xunit;
using Xunit.Abstractions;

namespace UpdaterServer.ApplicationVersion;


public class ApplicationVersionManagerIntegrationTests<TStartupModule> : UpdaterServerDomainTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IRepository<Application.Application> _applicationRepository;
    private readonly ApplicationVersionManager _applicationVersionManager;

    public ApplicationVersionManagerIntegrationTests()
    {
        _applicationRepository = GetRequiredService<IRepository<Application.Application>>();
        _applicationVersionManager = GetRequiredService<ApplicationVersionManager>();
    }

    [Fact]
    public async Task Should_Throw_When_ApplicationVersion_Exists()
    {
        var year = DateTime.Now.Year;
        var month = DateTime.Now.Month;
        await WithUnitOfWorkAsync(async () =>
        {
            var app = await _applicationRepository.FirstAsync();
            await _applicationVersionManager.CreateAsync(app, $"{year}.{month}.1", "Test version 1")
                .ShouldThrowAsync<BusinessException>();
        });

        await WithUnitOfWorkAsync(async () =>
        {
            var app = await _applicationRepository.FirstAsync();
            var v = await _applicationVersionManager.CreateAsync(app, $"{year}.{month}.13", "Test version 13");
            v.ApplicationId.ShouldBe(app.Id);
            v.VersionNumber.ShouldBe($"{year}.{month}.13");
        });
    }
}