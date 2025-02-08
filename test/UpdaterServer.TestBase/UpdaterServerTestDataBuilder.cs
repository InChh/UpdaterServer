using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UpdaterServer.File;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;

namespace UpdaterServer;

public class UpdaterServerTestDataSeedContributor(
    IGuidGenerator guidGenerator,
    IRepository<Application.Application> applicationRepository,
    IRepository<ApplicationVersion.ApplicationVersion> versionRepository,
    IRepository<FileMetadata> fileMetadataRepository)
    : IDataSeedContributor, ITransientDependency
{
    public async Task SeedAsync(DataSeedContext context)
    {
        /* Seed additional test data... */

        var apps =
            await Task.WhenAll(
                Enumerable.Range(0, UpdaterServerTestConsts.TestApplicationCount)
                    .Select(async i => await applicationRepository.InsertAsync(
                        new Application.Application(
                            guidGenerator.Create(),
                            $"TestApp{i + 1}",
                            $"Test Application {i + 1}"
                        )))
                    .ToList()
            );

        var fileMetadatas = new List<FileMetadata>();
        for (var i = 0; i < UpdaterServerTestConsts.TestFileMetadataCount; i++)
        {
            var fileMetadata = new FileMetadata(
                guidGenerator.Create(),
                $"TestFile{i + 1}",
                $"TestFile{i + 1} Hash",
                1024,
                $"https://test.com/TestFile{i + 1}"
            );
            fileMetadatas.Add(await fileMetadataRepository.InsertAsync(fileMetadata));
        }


        foreach (var app in apps)
        {
            for (var i = 0; i < UpdaterServerTestConsts.TestApplicationVersionCount; i++)
            {
                var year = DateTime.Now.Year;
                var month = DateTime.Now.Month;
                var versionNumber = $"{year}.{month}.{i + 1}";
                var applicationVersion = new ApplicationVersion.ApplicationVersion(guidGenerator.Create(), app.Id,
                    versionNumber,
                    $"Test version {versionNumber}");
                if (i % 2 == 0)
                {
                    applicationVersion.AddFiles(fileMetadatas.Select(metadata => metadata.Id).ToArray());
                }
                else
                {
                    applicationVersion.AddFiles(
                        fileMetadatas
                            .Skip(UpdaterServerTestConsts.TestFileMetadataCount / 2)
                            .Select(m => m.Id)
                            .ToArray());
                }

                await versionRepository.InsertAsync(applicationVersion);
            }
        }
    }
}