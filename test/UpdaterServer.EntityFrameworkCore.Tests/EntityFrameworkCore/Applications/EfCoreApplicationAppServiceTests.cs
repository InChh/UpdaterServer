using UpdaterServer.Application;
using Xunit;

namespace UpdaterServer.EntityFrameworkCore.Applications;

[Collection(UpdaterServerTestConsts.CollectionDefinitionName)]
public class EfCoreApplicationAppServiceTests : ApplicationAppServiceTests<UpdaterServerEntityFrameworkCoreTestModule>
{
}