using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.IdentifyDataLocks.Data.Model;

namespace SFA.DAS.IdentifyDataLocks.Data.Configurations
{
    public class EarningEventPriceEpisodeModelConfiguration : IEntityTypeConfiguration<EarningEventPriceEpisodeModel>
    {
        public void Configure(EntityTypeBuilder<EarningEventPriceEpisodeModel> builder)
        {
            builder.ToTable("EarningEventPriceEpisode", "Payments2");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName(@"Id").IsRequired();
            builder.Property(x => x.EarningEventId).HasColumnName(@"EarningEventId").IsRequired();
            builder.Property(x => x.TotalNegotiatedPrice1).HasColumnName(@"TotalNegotiatedPrice1");
            builder.Property(x => x.TotalNegotiatedPrice2).HasColumnName(@"TotalNegotiatedPrice2");
            builder.Property(x => x.TotalNegotiatedPrice3).HasColumnName(@"TotalNegotiatedPrice3");
            builder.Property(x => x.TotalNegotiatedPrice4).HasColumnName(@"TotalNegotiatedPrice4");
            builder.Property(x => x.StartDate).HasColumnName(@"StartDate");
            builder.Property(x => x.PlannedEndDate).HasColumnName(@"PlannedEndDate");
            builder.Property(x => x.ActualEndDate).HasColumnName(@"ActualEndDate");
        }
    }
}
