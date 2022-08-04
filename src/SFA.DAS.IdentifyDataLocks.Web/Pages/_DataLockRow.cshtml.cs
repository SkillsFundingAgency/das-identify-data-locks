using System;
using SFA.DAS.IdentifyDataLocks.Data.Model;
using SFA.DAS.IdentifyDataLocks.Domain;

namespace SFA.DAS.IdentifyDataLocks.Web.Pages
{
    public class DataLockRowModel
    {
        public string Heading { get; }

        public string ActiveDataLock { get; }

        public string ApprenticeValue => Extract(_period.ApprenticeshipDataMatch);

        public string IlrValue => Extract(_period.IlrEarningDataMatch);

        public bool IsLocked { get; }

        private readonly CollectionPeriod _period;
        private readonly Func<DataMatch, object> _valueExtractor;

        public DataLockRowModel(CollectionPeriod period, DataLockErrorCode dataLock, string heading, Func<DataMatch, object> valueExtractor)
        {
            Heading = heading;
            _period = period ?? throw new ArgumentNullException(nameof(period));
            _valueExtractor = valueExtractor ?? throw new ArgumentNullException(nameof(valueExtractor));
            IsLocked = _period.DataLockErrorCodes.Contains(dataLock);
            ActiveDataLock = IsLocked ? dataLock.ToString() : "-";
        }

        public DataLockRowModel(CollectionPeriod period, string heading, Func<DataMatch, object> value) : this(period, 0, heading, value)
        { }

        private string Extract(DataMatch data)
        {
            return data != null ? _valueExtractor(data)?.ToString() ?? "-" : "-";
        }
    }
}
