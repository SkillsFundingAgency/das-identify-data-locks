using System.Collections.Generic;

namespace SFA.DAS.LearnerDataMismatches.Domain
{
    public class LearnerReport
    {
        public (string Id, string Name) Learner { get; set; }
        public (string Id, string Name) Employer { get; set; }
        public (string Id, string Name) Provider { get; set; }
        public bool HasDataLocks { get; set; }
        public Dictionary<int, List<CollectionPeriod>> DataLocks { get; set; }
    }
}