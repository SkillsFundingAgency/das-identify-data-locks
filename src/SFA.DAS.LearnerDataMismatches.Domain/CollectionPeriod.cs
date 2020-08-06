using System;
using System.Collections.Generic;

namespace SFA.DAS.LearnerDataMismatches.Domain
{
    public class CollectionPeriod
    {
        public DataMatch Apprenticeship { get; set; }
        public DataMatch Ilr { get; set; }
        public List<DataLock> DataLocks { get; set; }
    }

    public class DataMatch
    {
        public long Ukprn { get; set; }
        public long Uln { get; set; }
        public short Standard { get; set; }
        public short Framework { get; set; }
        public short Program { get; set; }
        public short Pathway { get; set; }
        public decimal Decimal { get; set; }
        public DateTime PriceStart { get; set; }
        public short AimSequenceNumber { get; set; }
        public DateTime? PausedOn { get; set; }
        public DateTime? StoppedOn { get; set; }
        public DateTime? PlannedCompletion { get; set; }
        public string CompletionStatus { get; set; }
        public decimal Tnp1 { get; set; }
        public decimal Tnp2 { get; set; }
        public decimal Tnp3 { get; set; }
        public decimal Tnp4 { get; set; }

    }

    public enum DataLock
    {
        Dlock01 = 1,
    }
}
