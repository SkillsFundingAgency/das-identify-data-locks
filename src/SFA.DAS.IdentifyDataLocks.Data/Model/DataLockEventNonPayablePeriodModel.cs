using System;
using System.Collections.Generic;

namespace SFA.DAS.IdentifyDataLocks.Data.Model
{
    public class DataLockEventNonPayablePeriodModel
    {
        public long Id { get; set; }
        public virtual DataLockEventModel DataLockEvent { get; set; }
        public Guid DataLockEventId { get; set; }
        public Guid DataLockEventNonPayablePeriodId { get; set; }
        public decimal Amount { get; set; }
        public virtual List<DataLockEventNonPayablePeriodFailureModel> DataLockEventNonPayablePeriodFailures { get; set; }
    }
}