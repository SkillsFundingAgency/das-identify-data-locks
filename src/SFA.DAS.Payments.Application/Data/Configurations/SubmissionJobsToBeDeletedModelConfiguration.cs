using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.Application.Data.Configurations
{
    public class SubmissionJobsToBeDeletedModelConfiguration : IEntityTypeConfiguration<SubmissionJobsToBeDeletedModel>
    {
        public void Configure(EntityTypeBuilder<SubmissionJobsToBeDeletedModel> builder)
        {
            builder.HasNoKey();
        }
    }
}