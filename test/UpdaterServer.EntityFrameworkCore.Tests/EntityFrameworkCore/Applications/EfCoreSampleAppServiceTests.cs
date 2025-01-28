using UpdaterServer.Samples;
using Xunit;

namespace UpdaterServer.EntityFrameworkCore.Applications;

[Collection(UpdaterServerTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<UpdaterServerEntityFrameworkCoreTestModule>
{

}
