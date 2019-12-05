using Newtonsoft.Json.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ImdCloud
{
    public class VersionFileCreation : IVersionFileCreation
    {
        private readonly IClient client;

        public VersionFileCreation(IClient client)
        {
            this.client = client;
        }

        public async ValueTask<int> Execute(int versionId, int fileSize, string fileName, string userToken, CancellationToken token)
        {
            //# fileSize - Mandatory for QC, can't be updated

            var result = await client.Post<PostResponse>($"versions/{versionId}/files", JObject.Parse($@"{{
    ""filename"": ""{fileName}"",
    ""size"": {fileSize}
}}"), userToken, token);

            return result.FileId;
        }

        public class PostResponse
        { 
            public int FileId { get; set; }
        }
    }
}