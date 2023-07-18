using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SFA.DAS.Payments.Application.Repositories
{
    public class CurrentPeriodContextFactory : IDesignTimeDbContextFactory<PaymentsDataContext>
    {
        private readonly IConfiguration _configuration;

        public CurrentPeriodContextFactory(IConfiguration configuration) 
        {
            _configuration = configuration;
        }

        public PaymentsDataContext CreateDbContext() => CreateDbContext(Array.Empty<string>());

        public PaymentsDataContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PaymentsDataContext>();
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("CurrentPaymentsSqlConnectionString"))
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            return new PaymentsDataContext(optionsBuilder.Options);
        }
    }
}
