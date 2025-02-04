using UpdaterServer.Application;
using Xunit;

namespace UpdaterServer.EntityFrameworkCore.Domains;

[Collection(UpdaterServerTestConsts.CollectionDefinitionName)]
public class EfCoreApplicationManagerIntegrationTests : ApplicationManagerIntegrationTests<UpdaterServerEntityFrameworkCoreTestModule>
{
}
