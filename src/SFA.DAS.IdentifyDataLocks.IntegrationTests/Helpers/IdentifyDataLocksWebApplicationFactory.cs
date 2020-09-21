using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace SFA.DAS.IdentifyDataLocks.IntegrationTests
{
    public class IdentifyDataLocksWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
                .UseEnvironment("development")
                .ConfigureWebHostDefaults(webBuilder =>
                    webBuilder
                        .UseStartup<TStartup>()
                        .ConfigureAppConfiguration(configuration =>
                            configuration.AddJsonFile(AppSettingsLocation)));

        private static string AppSettingsLocation
        {
            get
            {
                var assemblyFilename = Assembly.GetExecutingAssembly().Location;
                var assemblyPath = System.IO.Path.GetDirectoryName(assemblyFilename);
                return System.IO.Path.Combine(assemblyPath, "appsettings.json");
            }
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder) =>
            builder.ConfigureServices(services =>
                services.AddRazorPages(options =>
                    options.Conventions.AllowAnonymousToPage("/learner")));
    }
}