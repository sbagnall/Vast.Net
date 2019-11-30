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

            return await client.Post("login", new Dictionary<string, string>()
            {
                { "userName", userCredentials.Username },
                { "password", userCredentials.Password },
                { "accountId", userCredentials.AccountId.ToString()  }
            }
            .Where(x => !string.IsNullOrEmpty(x.Value))
            .ToDictionary(x => x.Key, x => x.Value), null, token);
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
