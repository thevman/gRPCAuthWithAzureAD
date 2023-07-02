using System;
using System.Threading.Tasks;
using System.Net.Http;
using Grpc.Core;
using Grpc.Net.Client;
using grpcWithAuth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
namespace grpcClient
{
    class Program
    {
        private static IConfiguration configuration;
        static async Task Main(string[] args)
        {
            LoadAppSettings();
            AppContext.SetSwitch(
    "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var handler = new HttpClientHandler();
handler.ServerCertificateCustomValidationCallback = 
    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            //var authProvider = new DeviceCodeAuthProvider(configuration);
            //var token = await authProvider.GetAccessToken(new string[] {configuration["scope"]});
            var tokenAcquirerFactory = TokenAcquirerFactory.GetDefaultInstance();
            var acquirer = tokenAcquirerFactory.GetTokenAcquirer();
            AcquireTokenResult tokenResult = await acquirer.GetTokenForUserAsync(new[] { "https://graph.microsoft.com/.default" });
            var token = tokenResult.AccessToken;

            //var channel = GrpcChannel.ForAddress("https://localhost:8000");
            var channel = GrpcChannel.ForAddress("https://grpc-service.redpond-0b7c5ea7.canadacentral.azurecontainerapps.io",
            new GrpcChannelOptions { HttpHandler = handler });
            var client = new Greeter.GreeterClient(channel);
            
            var headers = new Metadata();
            headers.Add("Authorization", $"Bearer {token}");
            
            var request = new HelloRequest()
            {
                Name = "SpongeBob Vineet"
            };

            var reply = await client.SayHelloAsync(request, headers);
            Console.WriteLine(reply.Message);
        }

        static void LoadAppSettings()
        {
            configuration = new ConfigurationBuilder()
                            .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json")
                            .Build();
        }
    }
}
