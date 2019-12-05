using Amazon.S3;
using Moq;
using NUnit.Framework;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ImdCloud.Test
{
    [TestFixture]
    public class IngestMediaTest
    {
        private const string userToken = "user_token"; 

        private TestHelpers testHelpers;
        private ApiCredentials apiCredentials;

        private Mock<IAmazonS3> amazonS3;
        private Mock<IS3Credentials> s3Credentials;
        private Mock<IOrderCreation> orderCreation;
        private Mock<IVersionCreation> versionCreation;
        private Mock<IVersionFileCreation> versionFileCreation;
        

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



            //testHelpers.StubOrderCreation(orderId, token);
            //testHelpers.StubVersionCreation(orderId, versionId, token);
            testHelpers.StubVersionFileCreation(versionId, fileId, token);
            testHelpers.StubGetUploadCredentials(versionId, fileId, token);

            amazonS3 = new Mock<IAmazonS3>();
            s3Credentials = new Mock<IS3Credentials>();

            orderCreation = new Mock<IOrderCreation>();
            orderCreation.Setup(x => x.Execute(userToken, It.IsAny<CancellationToken>()))
               .ReturnsAsync(new OrderCreationResult()
               {
                   OrderId = orderId
               });

            versionCreation = new Mock<IVersionCreation>();
            versionCreation.Setup(x => x.Execute(It.IsAny<int>(), userToken, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new VersionCreationResult()
                {
                    Id = versionId,
                    VersionId = "HC-1545063645"
                });

            versionFileCreation = new Mock<IVersionFileCreation>();
            versionFileCreation.Setup(x => x.Execute(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            s3Credentials = new Mock<IS3Credentials>();
            s3Credentials.Setup(x => x.Execute(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new S3Credentials.S3CredentialsResult()
                {
                    BucketName = "bucket_name",
                    ObjectKey = "bucket_key"
                });

            target = new IngestMedia(
                orderCreation.Object,
                versionCreation.Object,
                versionFileCreation.Object,
                s3Credentials.Object,
                amazonS3.Object);
        }

        [Test]
        public void with_IMD_Cloud_operations_successful_it_returns_a_signed_S3_URL()
        {
            int fileSize = 1;
            string fileName = "file_name";

            var actual = target.GeneratePreSignedURL(fileSize, fileName, userToken, default);

            // TODO: implement
        }

        [Test]
        public void with_IMD_Cloud_operations_successful_it_creates_the_underlying_structure_and_memoizes_ids()
        {
            throw new NotImplementedException();
        }

    }
}
