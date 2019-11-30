using ImdCloud;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace NUnitTestProject1
{
    public class AuthenticationHelpers
    {
        private const string SUCCESS_RESPONSE = @"{
    ""token"": ""sampletoken"",
    ""accountName"": ""IMD Demo"",
    ""wasSuccessful"": true,
}";

        private const string FAILURE_RESPONSE = @"{
    ""reason"": ""Username or Password incorrect."",
    ""wasSuccessful"": false
}";

        private readonly ApiCredentials apiCredentials;

        public AuthenticationHelpers(ApiCredentials apiCredentials)
        {
            this.apiCredentials = apiCredentials;
        }

        public Mock<HttpMessageHandler> StubAuthentication(bool success = true, string username = "test@account.com", string password = "Password")
        {
            var response = success ? SUCCESS_RESPONSE : FAILURE_RESPONSE;

            var messageHandler = new Mock<HttpMessageHandler>();

            messageHandler.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(r => CheckRequest(r, username, password).GetAwaiter().GetResult()),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(response)
                });

            return messageHandler;
        }

        private async ValueTask<bool> CheckRequest(HttpRequestMessage message, string username, string password)
        {
            if (!(message.Content is StringContent))
            {
                return false;
            }

            var content = JsonConvert.DeserializeObject<IDictionary<string, string>>(
                await (message.Content as StringContent).ReadAsStringAsync());

            return message.RequestUri.ToString() == $"{apiCredentials.BaseUrl}/login"
                && content["userName"] == username && content["password"] == password
                && message.Headers.Accept.Contains(MediaTypeWithQualityHeaderValue.Parse("application/json"))
                && message.Headers.TryGetValues("ContentType", out IEnumerable<string> contentTypes) && contentTypes.Contains("application/json")
                && message.Headers.TryGetValues("X-Api-Key", out IEnumerable<string> keys) && keys.Contains(apiCredentials.Key)
                && message.Headers.TryGetValues("X-Api-Secret", out IEnumerable<string> secrets) && secrets.Contains(apiCredentials.Secret);
        }
    }
}
