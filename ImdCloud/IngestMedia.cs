using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ImdCloud
{
    public class IngestMedia : IIngestMedia
    {
        private readonly IAmazonS3 client;
        private readonly IOrderCreation orderCreation;
        private readonly IVersionCreation versionCreation;
        private readonly IVersionFileCreation versionFileCreation;
        private readonly IS3Credentials s3Credentials;

        public IngestMedia(
            IAmazonS3 client, 
            IS3Credentials s3Credentials,
            IOrderCreation orderCreation,
            IVersionCreation versionCreation,
            IVersionFileCreation versionFileCreation)
        {
            this.client = client;
            this.orderCreation = orderCreation;
            this.versionCreation = versionCreation;
            this.versionFileCreation = versionFileCreation;
            this.s3Credentials = s3Credentials;
        }

        public async ValueTask<string> GeneratePreSignedURL(int fileSize, string fileName, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var orderResult = await orderCreation.Execute(token);

            var versionResult = await versionCreation.Execute(orderResult.OrderId, token);

            var verionFileResult = await versionFileCreation.Execute(versionResult.Id, fileSize, fileName, token);


            var result = await s3Credentials.Execute(versionResult.Id,
                                                     verionFileResult,
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
