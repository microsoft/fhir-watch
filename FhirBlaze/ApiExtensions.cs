using FhirBlaze.SharedComponents;
using FhirBlaze.SharedComponents.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Authentication.WebAssembly.Msal.Models;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FhirBlaze
{
    public static class ApiExtensions
    {
        public static IServiceCollection AddDataverseService(
       this IServiceCollection services, Func<ApiConnection> connection)
        {
            var apiData = connection.Invoke();

            services.AddScoped(o =>
            {
                var provider = o.GetRequiredService<IAccessTokenProvider>();
                var navigationManager = o.GetRequiredService<NavigationManager>();

                return new BackendAuthorizationMessageHandler(provider, navigationManager, apiData);
            });

            services.AddHttpClient<DataverseService>(client => client.BaseAddress = new Uri(apiData.BaseUri + "/api/"))
                    .AddHttpMessageHandler<BackendAuthorizationMessageHandler>();

            return services;
        }
    }
}
