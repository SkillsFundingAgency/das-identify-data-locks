using Microsoft.EntityFrameworkCore;
using SFA.DAS.IdentifyDataLocks.Data.Configurations;
using SFA.DAS.IdentifyDataLocks.Data.Model;

namespace SFA.DAS.IdentifyDataLocks.Data.Repositories
{
    public class PaymentsDataContext : DbContext, IPaymentsDataContext
    {
        public virtual DbSet<ApprenticeshipModel> Apprenticeship { get; set; }
        public virtual DbSet<EarningEventModel> EarningEvent { get; set; }
        public virtual DbSet<DataLockEventModel> DataLockEvent { get; set; }

        public PaymentsDataContext(DbContextOptions<PaymentsDataContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("Payments2");
            modelBuilder.ApplyConfiguration(new ApprenticeshipModelConfiguration());
            modelBuilder.ApplyConfiguration(new ApprenticeshipPriceEpisodeModelConfiguration());
            modelBuilder.ApplyConfiguration(new ApprenticeshipPauseModelConfiguration());
            modelBuilder.ApplyConfiguration(new EarningEventModelConfiguration());
            modelBuilder.ApplyConfiguration(new EarningEventPriceEpisodeModelConfiguration());
            modelBuilder.ApplyConfiguration(new DataLockEventModelConfiguration());
            modelBuilder.ApplyConfiguration(new DataLockEventNonPayablePeriodModelConfiguration());
            modelBuilder.ApplyConfiguration(new DataLockEventNonPayablePeriodFailureModelConfiguration());
        }
    }
}
