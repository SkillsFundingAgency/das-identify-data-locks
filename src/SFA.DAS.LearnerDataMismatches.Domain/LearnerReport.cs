using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.LearnerDataMismatches.Domain
{
    public class LearnerReport
    {
        public (string Id, string Name) Learner { get; set; }
        public (string Id, string Name) Employer { get; set; }
        public (string Id, string Name) Provider { get; set; }
        public IEnumerable<CollectionPeriod> DataLocks { get; set; } = new List<CollectionPeriod>();

        public bool HasAnyDataLocks => DataLocks.Any();
    }
}