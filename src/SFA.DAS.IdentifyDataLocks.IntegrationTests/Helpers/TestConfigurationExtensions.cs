using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SFA.DAS.IdentifyDataLocks.Data.Repositories;
using SFA.DAS.IdentifyDataLocks.Web.Pages;

namespace SFA.DAS.IdentifyDataLocks.IntegrationTests.Helpers
{
    public static class TestConfigurationExtensions
    {
        public static void AddPages(this ServiceCollection services)
        {
            services.AddScoped<LearnerModel>();
        }

        public static IServiceCollection ConfigureMockService<T>(
            this IServiceCollection services,
            Func<IServiceProvider, T> service)
            where T : class
        {
            services.RemoveAll(typeof(T));
            services.AddScoped(service);
            return services;
        }
        
        public static IServiceCollection ConfigurePaymentsDbContext(this IServiceCollection services)
        {
            services.RemoveAll(typeof(PaymentsDataContext));
            services.RemoveAll(typeof(DbContextOptions<PaymentsDataContext>));

            var optionBuilder = new DbContextOptionsBuilder<PaymentsDataContext>();
            optionBuilder.UseInMemoryDatabase($"TestDB-{Guid.NewGuid()}", new InMemoryDatabaseRoot());

            services.AddSingleton(optionBuilder.Options);
            services.AddSingleton(new PaymentsDataContext(optionBuilder.Options));
            return services;
        }
        
        public static IServiceCollection ConfigurePaymentsAuditDataContext(this IServiceCollection services)
        {
            services.RemoveAll(typeof(PaymentsAuditDataContext));
            services.RemoveAll(typeof(DbContextOptions<PaymentsAuditDataContext>));

            var optionBuilder = new DbContextOptionsBuilder<PaymentsAuditDataContext>();
            optionBuilder.UseInMemoryDatabase($"TestDB-{Guid.NewGuid()}", new InMemoryDatabaseRoot());

            services.AddSingleton(optionBuilder.Options);
            services.AddSingleton(new PaymentsAuditDataContext(optionBuilder.Options));
            return services;
        }
    }
}