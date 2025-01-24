using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UpdaterServer.Data;
using Volo.Abp.DependencyInjection;

namespace UpdaterServer.EntityFrameworkCore;

public class EntityFrameworkCoreUpdaterServerDbSchemaMigrator
    : IUpdaterServerDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreUpdaterServerDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the UpdaterServerDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<UpdaterServerDbContext>()
            .Database
            .MigrateAsync();
    }
}
