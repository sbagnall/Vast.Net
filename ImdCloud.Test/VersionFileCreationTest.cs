using NUnit.Framework;
using System.Threading.Tasks;

namespace ImdCloud.Test
{
    [TestFixture]
    public class VersionFileCreationTest
    {
        private const string userToken = "user_token";

        private const int versionId = 1;
        private const int fileSize = 1000;
        private const string fileName = "fle_name";
        private const int fileId = 1;

        private ApiCredentials apiCredentials;
        private TestHelpers testHelpers;

        private IVersionFileCreation target;

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

            var client = new Client(testHelpers.StubVersionFileCreation(versionId, fileId, default).Object, apiCredentials);

            target = new VersionFileCreation(client);
        }

        [Test]
        public async Task When_()
        {
            var actual = await target.Execute(versionId, fileSize, fileName, userToken, default);

            Assert.AreEqual(fileId, actual);
        }
    }
}
