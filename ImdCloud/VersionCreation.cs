using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImdCloud;
using Newtonsoft.Json.Linq;

namespace ImdCloud
{
    public class VersionCreation : IVersionCreation
    {
        private const int MEDIA_FORMAT_ID = 5;
        private const int QC_FORMAT_NAME = 6;
        private const string LANGUAGE = "ENG";

        private readonly IClient client;

        public VersionCreation(IClient client)
        {
            this.client = client;
        }

        public async ValueTask<VersionCreationResult> Execute(int orderId, string userToken, CancellationToken token)
        {
            var rand = new Random(10_000);
            var randomElement = $"{DateTime.Now.Ticks}-{rand.Next()}";

            // Temporary, to be updated once we can detect it from an upload
            var seconds = 10;

            var result = await client.Post<PostResponse>("versions", JObject.Parse($@"{{
    ""orderId"": {orderId},
    ""versions"": [
        {{
            ""versionId"": ""VAST-Demo-{randomElement}"",
            ""versionIdName"": {{
                ""name"": ""Version ID""
            }},
            ""mediaFormat"": {{
                ""id"": {MEDIA_FORMAT_ID}
            }},
            ""qcFormat"": {{
                ""name"": {QC_FORMAT_NAME}
            }},
            ""versiontitle"": ""VAST Demo {randomElement}"",
            ""duration"": {{
                ""seconds"": {seconds}, 
                ""frames"": 0
            }},
            ""language"": {{
                ""name"": ""{LANGUAGE}""
            }}
        }}
    ]
}}"), userToken, token);

            //# {
            //#   "versions": [
            //#     {
            //#         "id": 1308194,
            //#         "versionId": "VAST-Demo-1544797853-9999"
            //#     }
            //#   ],
            //#   "validationResult": null
            //# }

            var version = result.Versions.First();
            return new VersionCreationResult()
            {
                Id = version.Id,
                VersionId = version.VersionId
            };

        }

        public class PostResponse
        {
            public VersionResponse[] Versions { get; set; }

            public class VersionResponse
            {
                public int Id { get; set; }

                public string VersionId { get; set; }
            }
        }
    }
}