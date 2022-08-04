using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.IdentifyDataLocks.Data.Model;

namespace SFA.DAS.IdentifyDataLocks.Data.Configurations
{
    public class ApprenticeshipModelConfiguration : IEntityTypeConfiguration<ApprenticeshipModel>
    {
        public void Configure(EntityTypeBuilder<ApprenticeshipModel> builder)
        {
            builder.ToTable("Apprenticeship", "Payments2");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName(@"Id").IsRequired();
            builder.Property(x => x.AccountId).HasColumnName(@"AccountId").IsRequired();
            builder.Property(x => x.Uln).HasColumnName(@"Uln").IsRequired();
            builder.Property(x => x.Ukprn).HasColumnName(@"Ukprn").IsRequired();
            builder.Property(x => x.EstimatedStartDate).HasColumnName(@"EstimatedStartDate").IsRequired();
            builder.Property(x => x.EstimatedEndDate).HasColumnName(@"EstimatedEndDate").IsRequired();
            builder.Property(x => x.StandardCode).HasColumnName(@"StandardCode");
            builder.Property(x => x.ProgrammeType).HasColumnName(@"ProgrammeType");
            builder.Property(x => x.FrameworkCode).HasColumnName(@"FrameworkCode");
            builder.Property(x => x.PathwayCode).HasColumnName(@"PathwayCode");
            builder.Property(x => x.StopDate).HasColumnName(@"StopDate");
            builder.Property(x => x.Status).HasColumnName(@"Status").IsRequired();

            builder.HasMany(x => x.ApprenticeshipPriceEpisodes)
                .WithOne().HasForeignKey(p => p.ApprenticeshipId);

            builder.HasMany(x => x.ApprenticeshipPauses)
                .WithOne()
                .HasForeignKey(p => p.ApprenticeshipId);
        }
    }
}
