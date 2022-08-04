using System;
using System.Collections.Generic;

namespace SFA.DAS.IdentifyDataLocks.Data.Model
{
    public class EarningEventModel
    {
        public long Id { get; set; }
        public Guid EventId { get; set; }
        public short AcademicYear { get; set; }
        public byte CollectionPeriod { get; set; }
        public long Ukprn { get; set; }
        public long LearnerUln { get; set; }
        public string LearnerReferenceNumber { get; set; }
        public string LearningAimReference { get; set; }
        public int? LearningAimStandardCode { get; set; }
        public int? LearningAimProgrammeType { get; set; }
        public int? LearningAimFrameworkCode { get; set; }
        public int? LearningAimPathwayCode { get; set; }
        public DateTime IlrSubmissionDateTime { get; set; }
        public virtual List<EarningEventPriceEpisodeModel> PriceEpisodes { get; set; } = new List<EarningEventPriceEpisodeModel>();
    }
}
