using NUnit.Framework;
using System.Threading.Tasks;

namespace ImdCloud.Test
{
    [TestFixture]
    public class OrderCreationTest
    {
        private const string userToken = "user_token";
        private const int orderId = 1;

        private ApiCredentials apiCredentials;
        private TestHelpers testHelpers;
        
        private IOrderCreation target;

        [SetUp]
        public void SetUp()
        {
            apiCredentials = new ApiCredentials()
            {
                BaseUrl = "http://test.com",
                Key = "key",
                Secret = "secret"
            };
            
            testHelpers = new TestHelpers(apiCredentials);

            var client = new Client(testHelpers.StubOrderCreation(orderId, default).Object, apiCredentials);

            target = new OrderCreation(client);
        }

        [Test]
        public async Task When_valid_returns_orderId()
        {
            var actual = await target.Execute(userToken, default);

            Assert.AreEqual(orderId, actual.OrderId);
        }
    }
}
