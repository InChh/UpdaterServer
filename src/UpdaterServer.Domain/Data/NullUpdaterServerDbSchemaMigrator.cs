using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace UpdaterServer.Data;

/* This is used if database provider does't define
 * IUpdaterServerDbSchemaMigrator implementation.
 */
public class NullUpdaterServerDbSchemaMigrator : IUpdaterServerDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
