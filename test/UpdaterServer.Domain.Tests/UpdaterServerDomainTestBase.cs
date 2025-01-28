using Volo.Abp.Modularity;

namespace UpdaterServer;

/* Inherit from this class for your domain layer tests. */
public abstract class UpdaterServerDomainTestBase<TStartupModule> : UpdaterServerTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
