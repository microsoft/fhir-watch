using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.PowerPlatform.Dataverse.Client;
using System.IO;

[assembly: FunctionsStartup(typeof(FhirWatch.Api.Startup))]

namespace FhirWatch.Api
{
    public class Startup : FunctionsStartup
    {
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            FunctionsHostBuilderContext context = builder.GetContext();

            builder.ConfigurationBuilder
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, "appsettings.json"), optional: true, reloadOnChange: false)
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, $"appsettings.{context.EnvironmentName}.json"), optional: true, reloadOnChange: false)
                .AddEnvironmentVariables();
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddScoped(o =>
            {
                var config = o.GetService<IConfiguration>();
                var options = new DataverseOptions();
                config.GetSection("Dataverse").Bind(options);

                return new ServiceClient($@"AuthType=ClientSecret;url={options.RootUri};ClientId={options.ClientId};ClientSecret={options.ClientSecret}");
            });
        }
    }
}
