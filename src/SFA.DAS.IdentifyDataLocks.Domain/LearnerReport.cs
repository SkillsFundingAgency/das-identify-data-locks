using System.Collections.Generic;

namespace SFA.DAS.IdentifyDataLocks.Domain
{
    public class LearnerReport
    {
        public (string Id, string Name) Learner { get; set; }
        public (string Id, string Name) Employer { get; set; }
        public (string Id, string Name) Provider { get; set; }
        public bool HasDataLocks { get; set; }
        public bool HasMultipleProviders { get; set; }
        public Dictionary<AcademicYear, List<CollectionPeriod>> DataLocks { get; set; }
    }
}