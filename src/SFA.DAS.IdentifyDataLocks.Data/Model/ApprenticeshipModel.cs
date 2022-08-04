using System;
using System.Collections.Generic;

namespace SFA.DAS.IdentifyDataLocks.Data.Model
{
    public class ApprenticeshipModel
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public long Uln { get; set; }
        public long Ukprn { get; set; }
        public DateTime EstimatedStartDate { get; set; }
        public DateTime EstimatedEndDate { get; set; }
        public long? StandardCode { get; set; }
        public int? ProgrammeType { get; set; }
        public int? FrameworkCode { get; set; }
        public int? PathwayCode { get; set; }
        public DateTime? StopDate { get; set; }
        public ApprenticeshipStatus Status { get; set; }
        public virtual List<ApprenticeshipPriceEpisodeModel> ApprenticeshipPriceEpisodes { get; set; }
        public virtual List<ApprenticeshipPauseModel> ApprenticeshipPauses { get; set; }
        public ApprenticeshipModel()
        {
            ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>();
            ApprenticeshipPauses = new List<ApprenticeshipPauseModel>();
        }
    }
}
