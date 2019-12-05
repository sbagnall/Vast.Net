using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ImdCloud
{
    public class Client : IClient
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ApiCredentials apiCredentials;

        public Client(IHttpClientFactory httpClientFactory, ApiCredentials apiCredentials)
        {
            this.httpClientFactory = httpClientFactory;
            this.apiCredentials = apiCredentials;
        }

        public async ValueTask<T> Get<T>(
            string path, 
            string backgroundUserToken, 
            CancellationToken token,
            IDictionary<string, string> @params = default) where T: new()
        {
            token.ThrowIfCancellationRequested();

            try
            {
                return await Query<T>(() =>
                {
                    var request = Connection(HttpMethod.Get, path, backgroundUserToken);

                    if (@params != default)
                    {
                        request.RequestUri = new Uri(QueryHelpers.AddQueryString(request.RequestUri.ToString(), @params));
                    }

                    return request;
                }, token);
            }
            catch (Exception e)
            {
                throw new Error($"Error while requesting GET {path}: {e.Message}");
            }
        }

        public async ValueTask<T> Post<T>(
            string path, 
            JObject payload, 
            string backgroundUserToken, 
            CancellationToken token) where T: new()
        {
            token.ThrowIfCancellationRequested();

            try
            {
                return await Query<T>(() =>
                {
                    var request = Connection(HttpMethod.Post, path, backgroundUserToken);

                    request.Content = new StringContent(JsonConvert.SerializeObject(payload, new JsonSerializerSettings()
                    {
                        ContractResolver = new DefaultContractResolver()
                        {
                            NamingStrategy = new CamelCaseNamingStrategy()
                        },
                        Formatting = Formatting.Indented
                    }));

                    return request;
                }, token);
            }
            catch (Exception e)
            {
                throw new Error($"Error while requesting POST {path}: {e.Message}");
            }
        }


        private async ValueTask<U> Query<U>(
                Func<HttpRequestMessage> requestFactory,
                CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            using (var client = httpClientFactory.CreateClient())
            using (var request = requestFactory())
            {
                using (var response = await client
                    .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, token)
                    .ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();
                    return JsonConvert.DeserializeObject<U>(await response.Content.ReadAsStringAsync());
                }
            }
        }

        private HttpRequestMessage Connection(HttpMethod method, string path, string backgroundUserToken)
        {
            var message = new HttpRequestMessage(method, new Uri(new Uri(apiCredentials.BaseUrl), path))
            {
                Headers = {
                    { HttpRequestHeader.ContentType.ToString(), "application/json" },
                    { HttpRequestHeader.Accept.ToString(), "application/json" },
                    { "X-Api-Key", apiCredentials.Key },
                    { "X-Api-Secret", apiCredentials.Secret },
                    {  "X-Api-Token", backgroundUserToken }
                }
            };

            return message;
        }
    }
}
