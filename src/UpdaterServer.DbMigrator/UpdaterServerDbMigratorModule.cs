using Microsoft.Extensions.DependencyInjection;
using UpdaterServer.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace UpdaterServer.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(UpdaterServerEntityFrameworkCoreModule),
    typeof(UpdaterServerApplicationContractsModule)
)]
public class UpdaterServerDbMigratorModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();  
        configuration["Redis:Configuration"] = configuration["ConnectionStrings:Redis"];  
    }
}
