using ImdCloud;
using NUnit.Framework;
using System.Threading.Tasks;

namespace NUnitTestProject1
{
    [TestFixture]
    public class ImdCloudTest
    {
        private readonly IAuthenticationHelpers authenticationHelpers;

        public ImdCloudTest(IAuthenticationHelpers authenticationHelpers)
        {
            this.authenticationHelpers = authenticationHelpers;
        }

        [Test]
        public async ValueTask When_1()
        {
            var httpMessageHandler = authenticationHelpers.StubAuthentication(true, "a@b.c", "secret");

            var client = new Client(httpMessageHandler.Object, new ApiCredentials());

            var imdCloud = new ImdCloud.ImdCloud(client);

            var actual = await imdCloud.Login(new UserCredentials()
            {
                Username = "a@b.c",
                Password = "secret",
            }, default);

            Assert.IsTrue(actual.ContainsKey("token"));
        }

        [Test]
        public void When_2()
        {
            var httpMessageHandler = authenticationHelpers.StubAuthentication(false, "a@b.c", "secret");

            var client = new Client(httpMessageHandler.Object, new ApiCredentials());

            var imdCloud = new ImdCloud.ImdCloud(client);

            Assert.Throws<Error>(async () => await imdCloud.Login(new UserCredentials()
            {
                Username = "a@b.c",
                Password = "secret",
            }, default));
        }
    }
}