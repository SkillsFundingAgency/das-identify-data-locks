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

        public static void ConfigureMockService<T>(
            this ServiceCollection services,
            Func<IServiceProvider, T> service)
            where T : class
        {
            services.RemoveAll(typeof(T));
            services.AddScoped(service);
        }

        public static void ConfigureMockServices<T1, T2>(
            this ServiceCollection services,
            Func<IServiceProvider, T1> service1,
            Func<IServiceProvider, T2> service2
            )
            where T1 : class where T2 : class
        {
            services.ConfigureMockService(service1);
            services.ConfigureMockService(service2);
        }
    }
}