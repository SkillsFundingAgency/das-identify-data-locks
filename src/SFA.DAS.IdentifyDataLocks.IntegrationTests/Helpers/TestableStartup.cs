using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.IdentifyDataLocks.Web;

namespace SFA.DAS.IdentifyDataLocks.IntegrationTests.Helpers
{
    public class TestableStartup : Startup
    {
        public TestableStartup(IConfiguration configuration, IWebHostEnvironment environment)
            : base(configuration, environment)
        {
        }

        protected override void ConfigureOperationalServices(IServiceCollection services)
        {
            /* Operational services impair testability, don't configure them */
        }
    }
}