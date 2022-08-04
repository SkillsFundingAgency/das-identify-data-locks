using System;
using System.Collections.Generic;

namespace SFA.DAS.IdentifyDataLocks.Data.Model
{
    public class DataLockEventModel
    {
        public long Id { get; set; }
        public Guid EventId { get; set; }
        public short AcademicYear { get; set; }
        public byte CollectionPeriod { get; set; }
        public long Ukprn { get; set; }
        public long LearnerUln { get; set; }
        public bool IsPayable { get; set; }
        public virtual List<DataLockEventNonPayablePeriodModel> NonPayablePeriods { get; set; } = new List<DataLockEventNonPayablePeriodModel>();
    }
}