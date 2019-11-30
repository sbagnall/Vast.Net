﻿using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ImdCloud
{
    public class Client : IClient
    {
        private readonly HttpMessageHandler messageHandler;
        private readonly ApiCredentials apiCredentials;

        public Client(HttpMessageHandler messageHandler, ApiCredentials apiCredentials)
        {
            this.messageHandler = messageHandler;
            this.apiCredentials = apiCredentials;
        }

        // this creates circular reference - pass token in s optional parameter instead
        //
        // We do not want users to see VAST demo orders on their accounts
        // so we use a specific user set up for a VAST Demo account on IMD Cloud
        //public void AsBackgroundUser()
        //{
            //background_account_token = ImdCloud.login!(
            //    username: ENV.fetch('IMD_USER'),
            //    password: ENV.fetch('IMD_USER_PASSWORD'),
            //    account_id: ENV.fetch('IMD_USERS_ACCOUNT_ID')
            //).fetch('token')
        //    
        //}

        public async ValueTask<IDictionary<string, string>> Get(
            string path, 
            IDictionary<string, string> @params, 
            string backgroundUserToken = default, 
            CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            try
            {
                return await Query(() =>
                {
                    var request = Connection(HttpMethod.Get, path, backgroundUserToken);

                    request.RequestUri = new Uri(QueryHelpers.AddQueryString(request.RequestUri.ToString(), @params));

                    return request;
                }, token);
            }
            catch (Exception e)
            {
                throw new Error($"Error while requesting GET {path}: {e.Message}");
            }
        }

        public async ValueTask<IDictionary<string, string>> Post(
            string path, 
            IDictionary<string, string> payload, 
            string backgroundUserToken = default, 
            CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            try
            {
                return await Query(() =>
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


        private async ValueTask<IDictionary<string, string>> Query(
                Func<HttpRequestMessage> requestFactory,
                CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            using (var client = new HttpClient(messageHandler))
            using (var request = requestFactory())
            {
                using (var response = await client
                    .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, token)
                    .ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();
                    return JsonConvert.DeserializeObject<IDictionary<string, string>>(await response.Content.ReadAsStringAsync());
                }
            }
        }

        private HttpRequestMessage Connection(HttpMethod method, string path, string backgroundUserToken = default)
        {
            var message = new HttpRequestMessage(method, new Uri(new Uri(apiCredentials.BaseUrl), path))
            {
                Headers = {
                    { HttpRequestHeader.ContentType.ToString(), "application/json" },
                    { HttpRequestHeader.Accept.ToString(), "application/json" },
                    { "X-Api-Key", apiCredentials.Key },
                    { "X-Api-Secret", apiCredentials.Secret }
                }
            };

            if (backgroundUserToken != default)
            {
                message.Headers.Add("X-Api-Token", backgroundUserToken);
            }

            return message;
        }
    }
}