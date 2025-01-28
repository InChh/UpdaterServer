using Xunit;

namespace UpdaterServer.EntityFrameworkCore;

[CollectionDefinition(UpdaterServerTestConsts.CollectionDefinitionName)]
public class UpdaterServerEntityFrameworkCoreCollection : ICollectionFixture<UpdaterServerEntityFrameworkCoreFixture>
{

}
