using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.LearnerDataMismatches.Domain
{
    public class LearnerReport
    {
        public ApprenticeshipModel ActiveApprenticeship { get; set; }
        public IEnumerable<CollectionPeriod> DataLocks { get; set; } = new List<CollectionPeriod>();
        public string ProviderName { get; set; }
        public string ProviderId { get; set; }
        public string EmployerName { get; set; }
        public string EmployerId { get; set; }
        public string LearnerName { get; set; }
        public bool HasAnyDataLocks => DataLocks.Any();
        public bool HasActiveApprenticeship => ActiveApprenticeship != null;
                        StoppedOn = activeApprenticeship.StopDate,
                        StoppedOn = x.PriceEpisodes.FirstOrDefault()?.ActualEndDate,
    }
}