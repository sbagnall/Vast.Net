﻿using NUnit.Framework;
using System.Threading.Tasks;

namespace ImdCloud.Test
{
    [TestFixture]
    public class VersionCreationTest
    {
        private const string userToken = "user_token";

        private const int orderId = 1;
        private const int versionId = 1;

        private ApiCredentials apiCredentials;
        private TestHelpers testHelpers;
        
        private IVersionCreation target;

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

            var client = new Client(testHelpers.StubVersionCreation(orderId, versionId, default).Object, apiCredentials);

            target = new VersionCreation(client);
        }

        [Test]
        public async Task When_valid_returns_versionId()
        {
            var actual = await target.Execute(orderId, userToken, default);

            Assert.AreEqual(versionId, actual.Id);
        }
    }
}
