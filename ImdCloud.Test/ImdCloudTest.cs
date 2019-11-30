using ImdCloud;
using NUnit.Framework;
using System.Threading.Tasks;

namespace ImdCloud.Test
{
    [TestFixture]
    public class ImdCloudTest
    {
        private AuthenticationHelpers authenticationHelpers;
        private ApiCredentials apiCredentials;

        [SetUp]
        public void SetUp()
        {
            apiCredentials = new ApiCredentials()
            {
                BaseUrl = "http://test.com",
                Key = "key",
                Secret = "secret"
            };

            authenticationHelpers = new AuthenticationHelpers(apiCredentials);
        }

        [Test]
        public async ValueTask It_returns_a_response_when_succesful()
        {
            var httpMessageHandler = authenticationHelpers.StubAuthentication(true, "a@b.c", "secret");

            var client = new Client(httpMessageHandler.Object, apiCredentials);

            var imdCloud = new ImdCloud(client);

            var actual = await imdCloud.Login(new UserCredentials()
            {
                Username = "a@b.c",
                Password = "secret",
            }, default);

            Assert.IsTrue(actual.ContainsKey("token"));
        }

        [Test]
        public void It_raises_an_error_when_unsuccesful()
        {
            var httpMessageHandler = authenticationHelpers.StubAuthentication(false, "a@b.c", "secret");

            var client = new Client(httpMessageHandler.Object, apiCredentials);

            var imdCloud = new ImdCloud(client);

            Assert.ThrowsAsync<Error>(async () => await imdCloud.Login(new UserCredentials()
            {
                Username = "a@b.c",
                Password = "secret",
            }, default));
        }
    }
}