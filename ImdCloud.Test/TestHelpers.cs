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

namespace ImdCloud.Test
{
    public class TestHelpers
    {
        private readonly ApiCredentials apiCredentials;

        public TestHelpers(ApiCredentials apiCredentials)
        {
            this.apiCredentials = apiCredentials;
        }

        public Mock<HttpMessageHandler> StubAuthentication(bool success = true, string username = "test@account.com", string password = "Password")
        {
            const string SUCCESS_RESPONSE = @"{
    ""token"": ""sampletoken"",
    ""accountName"": ""IMD Demo"",
    ""wasSuccessful"": true,
}";

            const string FAILURE_RESPONSE = @"{
    ""reason"": ""Username or Password incorrect."",
    ""wasSuccessful"": false
}";

            var response = success ? SUCCESS_RESPONSE : FAILURE_RESPONSE;

            var messageHandler = new Mock<HttpMessageHandler>();

            messageHandler.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(r => CheckRequest(
                    r, 
                    HttpMethod.Post, 
                    $"{apiCredentials.BaseUrl}/login",
                    (content) => content["userName"] == username && content["password"] == password,
                    null).GetAwaiter().GetResult()),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(response)
                });

            return messageHandler;
        }

        public void StubOrderCreation(int orderId, string token)
        {
            var response = $@"{{
    ""orderId"": { orderId }
}}";
            var messageHandler = new Mock<HttpMessageHandler>();

            messageHandler.Protected().Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r => CheckRequest(
                        r,
                        HttpMethod.Post,
                        $"{apiCredentials.BaseUrl}/orders",
                        (content) => true,
                        token).GetAwaiter().GetResult()),
                    ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage()
                    {
                        StatusCode = System.Net.HttpStatusCode.OK,
                        Content = new StringContent(response),
                    });
        }

        public void StubVersionCreation(int orderId, int versionId, string token)
        {
            var response = $@"{{
    ""versions"": [
        {{
            ""id"": { versionId },
            ""versionId"": ""HC-1545063645""
        }}
    ]
}}";
            var messageHandler = new Mock<HttpMessageHandler>();

            messageHandler.Protected().Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r => CheckRequest(
                        r,
                        HttpMethod.Post,
                        $"{apiCredentials.BaseUrl}/versions",
                        (content) => content["orderId"] == orderId.ToString(),
                        token).GetAwaiter().GetResult()),
                    ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage()
                    {
                        StatusCode = System.Net.HttpStatusCode.OK,
                        Content = new StringContent(response),
                    });
        }

        public void StubVersionFileCreation(int versionId, int fileId, string token)
        {
            var response = $@"{{
    ""fileId"": { fileId } 
}}";

            var messageHandler = new Mock<HttpMessageHandler>();

            messageHandler.Protected().Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r => CheckRequest(
                        r,
                        HttpMethod.Post,
                        $"{apiCredentials.BaseUrl}/versions/#{versionId}/files",
                        (content) => true,
                        token).GetAwaiter().GetResult()),
                    ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage()
                    {
                        StatusCode = System.Net.HttpStatusCode.OK,
                        Content = new StringContent(response),
                    });
        }

        public void StubGetUploadCredentials(int versionId, int fileId, string token)
        {
            var response = @"{
    ""key"" => ""1308308/6965249_sample.mp4"",
    ""token"" => ""FQoGZXIvYXdzEBAaDKoUxaydt5P9aHEQ5yLsAj6yU2DP4+yGyMbvyuND1D1BbTVEWWkKw+IxZ5moUcz3gdKomIbRZqWKaa+LOea77yPIIoRR7LBQkSoN51bSLBLmwceKkkG1ghQQznCgXndr7OqdNdWrv3i0cgLa1TMwZLiyzWwB0WNeknzJQlvR4kva9cGyHXC23aN2/BWqgT8uDOXavbSF1Ksz+pkkh5HZKO47hE34A8XAPGRVjSuNYt2n/diEx3TCDr5u8zn2+nH2poQcVldoIUvGeVb/mciFw6ZjXHck1eUaf2j/DMSrU9uYu72bppvVQTAUjG222svJFrGAo/oyBLzzLVrjI3OU4/2mLywDRBG4mpb8ANFc9GzJpmnWRLlMQ0aT0/vM8UZDk464aAFM63smtXfp+eVwdVhxZDr6ob3botuw8EiyKmMXRPMDO9KRT/EcSxdDXDIeaqJoM+LRYzitxVgHZPe+78HQGEhIUI3IaJZE10hmUKMoWevbKN/N5Jz4I28o5ere4AU="",
    ""accessKey"" => ""ASIAR4SI3ZJREDACTEDH"",
    ""secretAccessKey"" => ""REDACTEDAKVKbecmN6BiyCawF9vtEITurpRwIsdl"",
    ""bucket"" => ""st-ebus-media-ir"",
    ""region"" => ""us-east-1"",
    ""policy"" => ""{}""
}";

            var messageHandler = new Mock<HttpMessageHandler>();

            messageHandler.Protected().Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r => CheckRequest(
                        r,
                        HttpMethod.Get,
                        $"{apiCredentials.BaseUrl}/versions/{versionId}/files/{fileId}/uploadcredentialslogin",
                        (content) => true,
                        token).GetAwaiter().GetResult()),
                    ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage()
                    {
                        StatusCode = System.Net.HttpStatusCode.OK,
                        Content = new StringContent(response),
                    });
        }

       
        private async ValueTask<bool> CheckRequest(
          HttpRequestMessage message,
          HttpMethod method,
          string requestUrl,
          Func<IDictionary<string, string>, bool> checkBody,
          string token = null)
        {
            if (!(message.Content is StringContent))
            {
                return false;
            }

            var content = JsonConvert.DeserializeObject<IDictionary<string, string>>(
                await (message.Content as StringContent).ReadAsStringAsync());

            return message.Method == method
                && message.RequestUri.ToString() == requestUrl
                && checkBody(content)
                && message.Headers.Accept.Contains(MediaTypeWithQualityHeaderValue.Parse("application/json"))
                && message.Headers.TryGetValues("ContentType", out IEnumerable<string> contentTypes) && contentTypes.Contains("application/json")
                && message.Headers.TryGetValues("X-Api-Key", out IEnumerable<string> keys) && keys.Contains(apiCredentials.Key)
                && message.Headers.TryGetValues("X-Api-Secret", out IEnumerable<string> secrets) && secrets.Contains(apiCredentials.Secret)
                && (token == null || message.Headers.TryGetValues("X-Api-Token", out IEnumerable<string> tokens) && tokens.Contains(token));
        }
    }
}
