using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;

namespace ImdCloud
{
    public class IngestMedia
    {
        private readonly IAmazonS3 client;
        private readonly IS3Credentials s3Credentials;

        public IngestMedia(IAmazonS3 client, IS3Credentials s3Credentials)
        {
            this.client = client;
            this.s3Credentials = s3Credentials;
        }

        public async ValueTask<string> GeneratePreSignedURL(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var result = await s3Credentials.Execute(token);

            // TODO: implement

            GetPreSignedUrlRequest request = new GetPreSignedUrlRequest
            {
                BucketName = result.BucketName,
                Key = result.ObjectKey,
                Expires = DateTime.Now.AddHours(1)
            };

            return client.GetPreSignedURL(request);
        }
    }
}
