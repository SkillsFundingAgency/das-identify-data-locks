using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.IdentifyDataLocks.Data.Model;

namespace SFA.DAS.IdentifyDataLocks.Data.Configurations
{
    public class EarningEventModelConfiguration : IEntityTypeConfiguration<EarningEventModel>
    {
        public void Configure(EntityTypeBuilder<EarningEventModel> builder)
        {
            builder.ToTable("EarningEvent", "Payments2");
            builder.HasKey(x =>  x.Id);
            builder.Property(x => x.Id).HasColumnName(@"Id");
            builder.Property(x => x.EventId).HasColumnName(@"EventId");
            builder.Property(x => x.Ukprn).HasColumnName(@"Ukprn");
            builder.Property(x => x.CollectionPeriod).HasColumnName(@"CollectionPeriod");
            builder.Property(x => x.AcademicYear).HasColumnName(@"AcademicYear");
            builder.Property(x => x.LearnerReferenceNumber).HasColumnName(@"LearnerReferenceNumber");
            builder.Property(x => x.LearnerUln).HasColumnName(@"LearnerUln");
            builder.Property(x => x.LearningAimReference).HasColumnName(@"LearningAimReference");
            builder.Property(x => x.LearningAimProgrammeType).HasColumnName(@"LearningAimProgrammeType");
            builder.Property(x => x.LearningAimStandardCode).HasColumnName(@"LearningAimStandardCode");
            builder.Property(x => x.LearningAimFrameworkCode).HasColumnName(@"LearningAimFrameworkCode");
            builder.Property(x => x.LearningAimPathwayCode).HasColumnName(@"LearningAimPathwayCode");
            builder.Property(x => x.IlrSubmissionDateTime).HasColumnName(@"IlrSubmissionDateTime");

            builder.HasMany(x => x.PriceEpisodes).WithOne()
                .HasPrincipalKey(p => p.EventId)
                .HasForeignKey(pe => pe.EarningEventId);
        }
    }
}
