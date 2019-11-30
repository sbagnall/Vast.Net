using ImdCloud;
using Microsoft.Extensions.Options;
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
    public class AuthenticationHelpers : IAuthenticationHelpers
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

        public AuthenticationHelpers(IOptions<ApiCredentials> apiCredentials)
        {
            this.apiCredentials = apiCredentials.Value;
        }

        public Mock<HttpMessageHandler> StubAuthentication(bool success = true, string username = "test@account.com", string password = "Password")
        {
            var response = success ? SUCCESS_RESPONSE : FAILURE_RESPONSE;

            var messageHandler = new Mock<HttpMessageHandler>();

            messageHandler.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                It.Is<HttpRequestMessage>(r => CheckRequest(r, username, password)),
                It.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(response)
                });

            return messageHandler;
        }

        private bool CheckRequest(HttpRequestMessage message, string username, string password)
        {
            if (!(message.Content is StringContent))
            {
                return false;
            }

            var content = JsonConvert.DeserializeObject<IDictionary<string, string>>((message.Content as StringContent).ToString());

            return message.RequestUri.ToString() == $"{apiCredentials.BaseUrl}/login"
                && content["username"] == username && content["password"] == password
                && message.Headers.Accept.Contains(MediaTypeWithQualityHeaderValue.Parse("application/json"))
                && message.Headers.Contains("Content-Type") && message.Headers.GetValues("Content-Type").Contains("application/json")
                && message.Headers.Contains("X-Api-Key") && message.Headers.GetValues("X-Api-Key").Contains(apiCredentials.Key)
                && message.Headers.Contains("X-Api-Secret") && message.Headers.GetValues("X-Api-Secret").Contains(apiCredentials.Secret);
        }
    }
}
