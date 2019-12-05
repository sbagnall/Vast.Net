using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ImdCloud
{
    public class IngestMedia : IIngestMedia
    {
        private readonly IOrderCreation orderCreation;
        private readonly IVersionCreation versionCreation;
        private readonly IVersionFileCreation versionFileCreation;
        private readonly IS3Credentials s3Credentials;
        private readonly IAmazonS3 client;

        public IngestMedia(
            IOrderCreation orderCreation,
            IVersionCreation versionCreation,
            IVersionFileCreation versionFileCreation,
            IS3Credentials s3Credentials,
            IAmazonS3 client)
        {
            this.client = client;
            this.orderCreation = orderCreation;
            this.versionCreation = versionCreation;
            this.versionFileCreation = versionFileCreation;
            this.s3Credentials = s3Credentials;
        }

        public async ValueTask<string> GeneratePreSignedURL(int fileSize, string fileName, string userToken, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var orderResult = await orderCreation.Execute(userToken, token);

            var versionResult = await versionCreation.Execute(orderResult.OrderId, userToken, token);

            var verionFileResult = await versionFileCreation.Execute(versionResult.Id, fileSize, fileName, userToken, token);


            var result = await s3Credentials.Execute(versionResult.Id,
                                                     verionFileResult,
                                                     userToken,
                                                     token);

            // TODO: implement

            GetPreSignedUrlRequest request = new GetPreSignedUrlRequest
            {
                BucketName = result.BucketName,
                Key = result.ObjectKey,
                Expires = DateTime.Now.AddHours(1)
            };

            return client.GetPreSignedURL(request);

            //client.PutPre
        }
    }
}
