using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.IdentifyDataLocks.Web;

namespace SFA.DAS.IdentifyDataLocks.IntegrationTests
{
    public class TestableStartup : Startup
    {
        public TestableStartup(IConfiguration configuration)
            : base(configuration)
        {
        }

        protected override void ConfigureOperationalServices(IServiceCollection services)
        {
            /* Operational services impair testability, don't configure them */
        }
    }
}