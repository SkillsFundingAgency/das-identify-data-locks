using System;

namespace SFA.DAS.IdentifyDataLocks.Data.Model
{
    public class ApprenticeshipPauseModel
    {
        public long Id { get; set; }
        public long ApprenticeshipId { get; set; }
        public DateTime PauseDate   { get; set; }
        public DateTime? ResumeDate { get; set; }
    }
}
