using NUnit.Framework;
using System.Threading.Tasks;

namespace ImdCloud.Test
{
    [TestFixture]
    public class OrderCreationTest
    {
        [Test]
        public async Task When_valid_returns_orderId()
        {
            var orderId = 1;

            var apiCredentials = new ApiCredentials()
            {
                BaseUrl = "http://test.com",
                Key = "key",
                Secret = "secret"
            };

            var testHelpers = new TestHelpers(apiCredentials);

            var client = new Client(testHelpers.StubOrderCreation(orderId, default).Object, apiCredentials);

            var target = new OrderCreation(client);

            var actual = await target.Execute(default);

            Assert.AreEqual(orderId, actual.OrderId);
        }
    }
}
