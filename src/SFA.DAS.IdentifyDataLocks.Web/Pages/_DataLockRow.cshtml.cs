using SFA.DAS.IdentifyDataLocks.Domain;
using System;

namespace SFA.DAS.IdentifyDataLocks.Web.Pages
{
    public class DataLockRowModel
    {
        public string Heading { get; }

        public string ActiveDataLock { get; }

        public string ApprenticeValue =>
            valueExtractor?.Invoke(period.Apprenticeship)?.ToString();

        public string IlrValue =>
            valueExtractor?.Invoke(period.Ilr)?.ToString();

        public bool IsLocked { get; }

        private readonly CollectionPeriod period;
        private readonly Func<DataMatch, object?> valueExtractor;

        public DataLockRowModel(
            CollectionPeriod period,
            DataLock dataLock,
            string heading,
            Func<DataMatch, object?> value)
        {
            Heading = heading;
            this.period = period;
            valueExtractor = value;
            IsLocked = this.period.DataLocks.Contains(dataLock);
            ActiveDataLock = IsLocked ? dataLock.ToString() : "-";
        }

        public DataLockRowModel(
            CollectionPeriod period,
            string heading,
            Func<DataMatch, object?> value)
            : this(period, 0, heading, value)
        { }
    }
}
