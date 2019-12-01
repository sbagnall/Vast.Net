using NUnit.Framework;
using System.Threading.Tasks;

namespace ImdCloud.Test
{
    [TestFixture]
    public class VersionCreationTest
    {
        [Test]
        public async Task When_valid_returns_orderId()
        {
            var orderId = 1;
            var versionId = 1;

            var apiCredentials = new ApiCredentials()
            {
                BaseUrl = "http://test.com",
                Key = "key",
                Secret = "secret"
            };

            var testHelpers = new TestHelpers(apiCredentials);

            var client = new Client(testHelpers.StubVersionCreation(orderId, versionId, default).Object, apiCredentials);

            var target = new VersionCreation(client);

            var actual = await target.Execute(orderId, default);

            Assert.AreEqual(versionId, actual.Id);
        }
    }
}
