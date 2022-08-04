using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.IdentifyDataLocks.Data.Model;

namespace SFA.DAS.IdentifyDataLocks.Data.Configurations
{
    public class DataLockEventModelConfiguration : IEntityTypeConfiguration<DataLockEventModel>
    {
        public void Configure(EntityTypeBuilder<DataLockEventModel> builder)
        {
            builder.ToTable("DataLockEvent", "Payments2");
            builder.HasKey(x => x.EventId);
            builder.Property(x => x.Id).HasColumnName(@"Id").IsRequired();
            builder.Property(x => x.EventId).HasColumnName(@"EventId").IsRequired();
            builder.Property(x => x.Ukprn).HasColumnName(@"Ukprn").IsRequired();
            builder.Property(x => x.CollectionPeriod).HasColumnName(@"CollectionPeriod").IsRequired();
            builder.Property(x => x.AcademicYear).HasColumnName(@"AcademicYear").IsRequired();
            builder.Property(x => x.LearnerUln).HasColumnName(@"LearnerUln");
            builder.Property(x => x.IsPayable).HasColumnName(@"IsPayable");

            builder.HasMany(x => x.NonPayablePeriods)
                .WithOne(y => y.DataLockEvent)
                .HasForeignKey(p => p.DataLockEventId);
        }
    }
}