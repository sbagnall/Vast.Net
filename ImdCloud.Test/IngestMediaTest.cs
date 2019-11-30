using NUnit.Framework;
using System;

namespace ImdCloud.Test
{
    [TestFixture]
    public class IngestMediaTest
    {
        private TestHelpers testHelpers;
        private ApiCredentials apiCredentials;

        private IngestMedia target;

        [SetUp]
        public void Setup()
        {
            var token = "ea9f4b10-4592-4c87-88b9-ee09060b7d66";
            var orderId = 1_309_116;
            var versionId = 1_309_130;
            var fileId = 6_965_257;

            apiCredentials = new ApiCredentials()
            {
                BaseUrl = "http://test.com",
                Key = "key",
                Secret = "secret"
            };

            testHelpers = new TestHelpers(apiCredentials);

            testHelpers.StubOrderCreation(orderId, token);
            testHelpers.StubVersionCreation(orderId, versionId, token);
            testHelpers.StubVersionFileCreation(versionId, fileId, token);
            testHelpers.StubGetUploadCredentials(versionId, fileId, token);

        }

        [Test]
        public void with_IMD_Cloud_operations_successful_it_returns_a_signed_S3_URL()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void with_IMD_Cloud_operations_successful_it_creates_the_underlying_structure_and_memoizes_ids()
        {
            throw new NotImplementedException();
        }

    }
}
