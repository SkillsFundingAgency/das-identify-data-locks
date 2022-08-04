using System;

namespace SFA.DAS.IdentifyDataLocks.Data.Model
{
    public class DataLockEventNonPayablePeriodFailureModel
    {
        public virtual long Id { get; set; }
        public virtual Guid DataLockEventNonPayablePeriodId { get; set; }
        public virtual DataLockErrorCode DataLockFailure { get; set; }
        public virtual long? ApprenticeshipId { get; set; }
        public virtual DataLockEventNonPayablePeriodModel DataLockEventNonPayablePeriod { get; set; }
    }
}