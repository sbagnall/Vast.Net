using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ImdCloud
{
    public class ImdCloud
    {
        private IClient client;

        public ImdCloud(IClient client)
        {
            this.client = client;
        }

        private async ValueTask<IDictionary<string, string>> LoginImpl(UserCredentials userCredentials, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            string request = string.Empty;
            
            if (userCredentials.AccountId == null)
            {
                request = $@"{{
    ""userName"": ""{userCredentials.Username}"",
    ""password"": ""{userCredentials.Password}""
}}";
            }
            else
            {
                request = $@"{{
    ""userName"": ""{userCredentials.Username}"",
    ""password"": ""{userCredentials.Password}"",
    ""accountId"": ""{userCredentials.AccountId.ToString()}""
}}";
            }

            return await client.Post<Dictionary<string, string>>("login", JObject.Parse(request));
        }

        public async ValueTask<IDictionary<string, string>> Login(UserCredentials userCredentials, CancellationToken token)
        {
            var response = await LoginImpl(userCredentials, token);

            if (!bool.Parse(response["wasSuccessful"]))
            {
                throw new Error($"Login error: {response["reason"]}");
            }

            return response;
        }
    }
}
