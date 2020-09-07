using System;
using System.Collections.Generic;

namespace SFA.DAS.IdentifyDataLocks.Domain
{
    public class CollectionPeriod : IComparable<CollectionPeriod>, IComparable
    {
        public Period Period { get; set; }
        public DataMatch Apprenticeship { get; set; }
        public DataMatch Ilr { get; set; }
        public List<DataLock> DataLocks { get; set; } = new List<DataLock>();

        public int CompareTo(CollectionPeriod other) =>
            Period.CompareTo(other.Period);

        public int CompareTo(object obj) =>
            obj is CollectionPeriod other
                ? CompareTo(other)
                : throw new ArgumentException(
                    $"Object is not a {nameof(CollectionPeriod)}");
    }
}