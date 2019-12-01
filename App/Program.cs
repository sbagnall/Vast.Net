using Amazon.S3;
using ImdCloud;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace App
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseStartup<Startup>()
                .Build();
            host.Run();
        }
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            // needed to resolve IHttpClientFactory
            services.AddHttpClient();

            ConfigureFromEnvironment(services);

            services.AddTransient<IOrderCreation, OrderCreation>();
            services.AddTransient<IVersionCreation, VersionCreation>();
            services.AddTransient<IVersionFileCreation, VersionFileCreation>();
            services.AddTransient<IClient, Client>();
            services.AddTransient<IS3Credentials, S3Credentials>();
            services.AddTransient<IAmazonS3, AmazonS3Client>();
            services.AddTransient<IIngestMedia, IngestMedia>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvcWithDefaultRoute();
        }

        private void ConfigureFromEnvironment(IServiceCollection services)
        {
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables("VAST_")
                .Build();

            var variables = config.GetChildren();

            var apiCredentials = new ApiCredentials()
            {
                BaseUrl = variables.Single(x => x.Key == "BaseUrl").Value,
                Key = variables.Single(x => x.Key == "Key").Value,
                Secret = variables.Single(x => x.Key == "Secret").Value,
            };

            services.AddTransient((x) => apiCredentials);
        }
    }
}
