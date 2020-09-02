using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SFA.DAS.LearnerDataMismatches.Web.Pages;
using System;

namespace SFA.DAS.LearnerDataMismatches.IntegrationTests
{
    public static class TestConfigurationExtensions
    {
        public static void AddPages(this ServiceCollection services)
        {
            services.AddScoped<LearnerModel>();
        }

        public static ServiceCollection ConfigureMockService<T>(
            this ServiceCollection services,
            Func<IServiceProvider, T> service)
            where T : class
        {
            services.RemoveAll(typeof(T));
            services.AddScoped(service);
            return services;
        }
    }
}