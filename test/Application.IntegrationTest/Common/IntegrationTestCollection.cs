using Xunit;

namespace Application.IntegrationTest.Common;

[CollectionDefinition("IntegrationTestCollection")]
public class IntegrationTestCollection : ICollectionFixture<IntegrationTestBase>
{

}
