using System.Threading.Tasks;

namespace UpdaterServer.Data;

public interface IUpdaterServerDbSchemaMigrator
{
    Task MigrateAsync();
}
