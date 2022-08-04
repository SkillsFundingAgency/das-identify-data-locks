//using System.IO;
//using System.Reflection;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Mvc.Testing;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;

//namespace SFA.DAS.IdentifyDataLocks.IntegrationTests.Helpers
//{
//    public class IdentifyDataLocksWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
//    {
//        protected override IHostBuilder CreateHostBuilder()
//        {
//            return Host.CreateDefaultBuilder()
//                .UseEnvironment("development")
//                .ConfigureWebHostDefaults(webBuilder =>
//                    webBuilder
//                        .UseStartup<TStartup>()
//                        .ConfigureAppConfiguration(configuration =>
//                            configuration.AddJsonFile(AppSettingsLocation)));
//        }

//        private static string AppSettingsLocation
//        {
//            get
//            {
//                var assemblyFilename = Assembly.GetExecutingAssembly().Location;
//                var assemblyPath = Path.GetDirectoryName(assemblyFilename);
//                return Path.Combine(assemblyPath, "appsettings.json");
//            }
//        }

//        protected override void ConfigureWebHost(IWebHostBuilder builder)
//        {
//            builder.ConfigureServices(services =>
//                services.AddRazorPages(options =>
//                    options.Conventions.AllowAnonymousToPage("/learner")));
//        }
//    }
//}