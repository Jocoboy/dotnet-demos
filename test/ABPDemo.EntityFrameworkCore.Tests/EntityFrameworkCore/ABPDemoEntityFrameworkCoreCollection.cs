using Xunit;

namespace ABPDemo.EntityFrameworkCore;

[CollectionDefinition(ABPDemoTestConsts.CollectionDefinitionName)]
public class ABPDemoEntityFrameworkCoreCollection : ICollectionFixture<ABPDemoEntityFrameworkCoreFixture>
{

}
