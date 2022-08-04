using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.IdentifyDataLocks.Data.Model;

namespace SFA.DAS.IdentifyDataLocks.Data.Configurations
{
    public class DataLockEventNonPayablePeriodModelConfiguration : IEntityTypeConfiguration<DataLockEventNonPayablePeriodModel>
    {
        public void Configure(EntityTypeBuilder<DataLockEventNonPayablePeriodModel> builder)
        {
            builder.ToTable("DataLockEventNonPayablePeriod", "Payments2");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName(@"Id").IsRequired();
            builder.Property(x => x.DataLockEventId).HasColumnName(@"DataLockEventId").IsRequired();
            builder.Property(x => x.DataLockEventNonPayablePeriodId).HasColumnName(@"DataLockEventNonPayablePeriodId").IsRequired();
            builder.Property(x => x.Amount).HasColumnName(@"Amount").IsRequired();

            builder.HasOne(x => x.DataLockEvent)
                .WithMany(dl => dl.NonPayablePeriods)
                .HasForeignKey(x => x.DataLockEventId);

            builder.HasMany(x => x.DataLockEventNonPayablePeriodFailures)
                .WithOne(y => y.DataLockEventNonPayablePeriod)
                .HasPrincipalKey(y => y.DataLockEventNonPayablePeriodId);
        }
    }
}