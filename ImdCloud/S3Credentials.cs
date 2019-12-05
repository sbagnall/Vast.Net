using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ImdCloud
{
    public class S3Credentials : IS3Credentials
    {
        private IClient client;
        
        public S3Credentials(IClient client)
        {
            this.client = client;
        }

        public async ValueTask<S3CredentialsResult> Execute(int versionId, int fileId, string userToken, CancellationToken token)
        {
            var result = await client.Get<Dictionary<string, string>>(
                $"versions/{versionId}/files/{fileId}/uploadcredentials", userToken, token);

            return Map(result);
        }

        private S3CredentialsResult Map(IDictionary<string, string> hash)
        {
            return new S3CredentialsResult()
            {
                BucketName = hash["bucketName"],
                ObjectKey = hash["objectKey"]
            };
        }

        public class S3CredentialsResult
        {
            public string BucketName { get; set; }
            public string ObjectKey { get; set; }
        }
    }
}
