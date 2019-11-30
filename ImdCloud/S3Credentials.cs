using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ImdCloud
{
    public class S3Credentials : IS3Credentials
    {
        private IClient client;
        private int versionId;
        private int fileId;

        public S3Credentials(IClient client, int versionId, int fileId)
        {
            this.client = client;
            this.versionId = versionId;
            this.fileId = fileId;
        }

        public async ValueTask<S3CredentialsResult> Execute(CancellationToken token)
        {
            var result = await client.Get($"versions/{versionId}/files/{fileId}/uploadcredentials", token: token);

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
            public string ObjectKey { get; internal set; }
        }
    }
}
