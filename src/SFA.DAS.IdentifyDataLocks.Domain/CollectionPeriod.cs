using System;
using System.Collections.Generic;
using SFA.DAS.IdentifyDataLocks.Data.Model;

namespace SFA.DAS.IdentifyDataLocks.Domain
{
    public class CollectionPeriod : IComparable<CollectionPeriod>, IComparable
    {
        public Period Period { get; set; }
        public DataMatch ApprenticeshipDataMatch { get; set; }
        public DataMatch IlrEarningDataMatch { get; set; }
        public List<DataLockErrorCode> DataLockErrorCodes { get; set; } = new List<DataLockErrorCode>();

        public int CompareTo(CollectionPeriod other)
        {
            return Period.CompareTo(other.Period);
        }

        public int CompareTo(object obj)
        {
            return obj is CollectionPeriod other
                ? CompareTo(other)
                : throw new ArgumentException($"Object is not a {nameof(CollectionPeriod)}");
        }
    }
}