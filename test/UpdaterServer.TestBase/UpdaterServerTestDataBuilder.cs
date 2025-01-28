using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;

namespace UpdaterServer;

public class UpdaterServerTestDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IGuidGenerator _guidGenerator;
    private readonly IRepository<Application.Application> _applicationRepository;
    private readonly IRepository<ApplicationVersion.ApplicationVersion> _versionRepository;

    public UpdaterServerTestDataSeedContributor(IGuidGenerator guidGenerator,
        IRepository<Application.Application> applicationRepository,
        IRepository<ApplicationVersion.ApplicationVersion> versionRepository)
    {
        _guidGenerator = guidGenerator;
        _applicationRepository = applicationRepository;
        _versionRepository = versionRepository;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        /* Seed additional test data... */

        var app1 = await _applicationRepository.InsertAsync(
            new Application.Application(_guidGenerator.Create(), "TestApp1", "Test Application 1"));

        var app2 = await _applicationRepository.InsertAsync(
            new Application.Application(_guidGenerator.Create(), "TestApp2", "Test Application 2"));

        var app3 = await _applicationRepository.InsertAsync(
            new Application.Application(_guidGenerator.Create(), "TestApp3", "Test Application 3"));

        var app4 = await _applicationRepository.InsertAsync(
            new Application.Application(_guidGenerator.Create(), "TestApp4", "Test Application 4"));
        List<Application.Application> apps = [app1, app2, app3, app4];

        foreach (var application in apps)
        {
            for (var i = 1; i <= 12; i++)
            {
                for (var j = 1; j <= 10; j++)
                {
                    var versionNumber = $"2025.{i}.{j}";
                    await _versionRepository.InsertAsync(
                        new ApplicationVersion.ApplicationVersion(_guidGenerator.Create(), application.Id, versionNumber,
                            $"Test version {versionNumber}"));
                }
            }
        }
    }
}