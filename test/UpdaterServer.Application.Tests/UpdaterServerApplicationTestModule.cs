using Volo.Abp.Modularity;

namespace UpdaterServer;

[DependsOn(
    typeof(UpdaterServerApplicationModule),
    typeof(UpdaterServerDomainTestModule)
)]
public class UpdaterServerApplicationTestModule : AbpModule
{

}
