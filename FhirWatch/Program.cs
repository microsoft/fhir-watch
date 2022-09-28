using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FhirWatch.SharedComponents;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using FhirWatch.Graph;
using System.Net.Http;
using Blazored.Modal;
using FhirWatch.SharedComponents.Services;
using System.Net;

namespace FhirWatch
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            
            // Get Backend API configuration
            var apiConnection = new ApiConnection();
            builder.Configuration.Bind("Api", apiConnection);
            if (string.IsNullOrEmpty(apiConnection.BaseUri))
                apiConnection.BaseUri = builder.HostEnvironment.BaseAddress;
            
            builder.RootComponents.Add<App>("#app");

            // Register API Backend
            builder.Services.AddDataverseService(() =>
            {                
                return apiConnection;
            });

            builder.Services.AddHttpClient<GraphClientFactory>(sp => new HttpClient { BaseAddress = new Uri("https://graph.microsoft.com") });
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddMsalAuthentication<RemoteAuthenticationState, RemoteUserAccount>(options =>
            {
                var scopes = builder.Configuration.GetValue<string>("GraphScopes");
                if (string.IsNullOrEmpty(scopes))
                {
                    Console.WriteLine("WARNING: No permission scopes were found in the GraphScopes app setting. Using default User.Read.");
                    scopes = "User.Read";
                }

                foreach (var scope in scopes.Split(';'))
                {
                    Console.WriteLine($"Adding {scope} to requested permissions");
                    options.ProviderOptions.DefaultAccessTokenScopes.Add(scope);
                }

                builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
                options.ProviderOptions.LoginMode = "redirect";
            })
            .AddAccountClaimsPrincipalFactory<RemoteAuthenticationState, RemoteUserAccount, GraphUserAccountFactory>();

            builder.Services.AddScoped<GraphClientFactory>();
            if (builder.Configuration.GetValue<bool>("UseGraphir"))
            {
                builder.Services.AddGraphirService(() =>
                {
                    var graphir = new GraphirConnection();
                    builder.Configuration.Bind("Graphir", graphir);
                    return graphir;
                });
            }
            else
            {
                builder.Services.AddFhirService(() =>
                {
                    var fhir = new FhirDataConnection();
                    builder.Configuration.Bind("FhirConnection", fhir);
                    return fhir;
                });
            }

            builder.Services.AddBlazoredModal();

            await builder.Build().RunAsync();
        }
    }
}