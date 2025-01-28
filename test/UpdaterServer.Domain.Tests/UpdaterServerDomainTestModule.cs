using Volo.Abp.Modularity;

namespace UpdaterServer;

[DependsOn(
    typeof(UpdaterServerDomainModule),
    typeof(UpdaterServerTestBaseModule)
)]
public class UpdaterServerDomainTestModule : AbpModule
{

}
