using System.Globalization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using SFA.DAS.Configuration.AzureTableStorage;
using System.Reflection;

namespace SFA.DAS.IdentifyDataLocks.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var cultureInfo = new CultureInfo("en-GB");

            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(configBuilder =>
                {
                    var config = configBuilder.Build();
                    var assemblyName = Assembly.GetAssembly(typeof(Startup)).GetName().Name;
                    configBuilder.AddAzureTableStorage(options =>
                     {
                         options.ConfigurationKeys = new[] { assemblyName };
                         options.StorageConnectionString = config["ConfigurationStorageConnectionString"];
                         options.EnvironmentName = config["EnvironmentName"];
                         options.PreFixConfigurationKeys = false;
                     });
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("https://localhost:44347/");
                });
    }
}
