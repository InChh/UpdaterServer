using Volo.Abp.Modularity;

namespace UpdaterServer;

public abstract class UpdaterServerApplicationTestBase<TStartupModule> : UpdaterServerTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
