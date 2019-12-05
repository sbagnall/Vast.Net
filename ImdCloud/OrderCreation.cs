using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ImdCloud
{
    public class OrderCreation : IOrderCreation
    {
        private readonly IClient client;

        public OrderCreation(IClient client)
        {
            this.client = client;
        }

        public async ValueTask<OrderCreationResult> Execute(string userToken, CancellationToken token)
        {
            var rand = new Random(10_000);

            return await client.Post<OrderCreationResult>("orders", JObject.Parse($@"{{
    ""order"": ""VAST-Demo-{DateTime.Now.Ticks}-{rand.Next()}"",
    ""client"": {{
        ""id"": ""VAST Demo"",
        ""name"": ""VAST Demo"",
        ""brand"": {{
            ""id"": ""VAST Demo"",
            ""name"": ""VAST Demo"",
            ""product"": {{
                ""id"": ""VAST Demo"",
                ""name"": ""VAST Demo""
            }}
        }}
    }},
    ""agency"": {{
        ""id"": ""VAST Demo"",
        ""name"": ""VAST Demo""
    }}
}}"), userToken, token);
        }
    }
}