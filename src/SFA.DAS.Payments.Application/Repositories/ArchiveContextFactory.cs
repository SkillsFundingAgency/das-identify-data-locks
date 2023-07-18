using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SFA.DAS.Payments.Application.Repositories
{
    public class ArchiveContextFactory : IDesignTimeDbContextFactory<PaymentsDataContext>
    {
        private readonly IConfiguration _configuration;

        public ArchiveContextFactory(IConfiguration configuration) 
        {
            _configuration = configuration;
        }

        public PaymentsDataContext CreateDbContext() => CreateDbContext(Array.Empty<string>());

        public PaymentsDataContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PaymentsDataContext>();
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("ArchivePaymentsSqlConnectionString"))
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            return new PaymentsDataContext(optionsBuilder.Options);
        }
    }
}
