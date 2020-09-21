using SFA.DAS.IdentifyDataLocks.Domain;
using System;

namespace SFA.DAS.IdentifyDataLocks.Web.Pages
{
    public class DataLockRowModel
    {
        public string Heading { get; }

        public string ActiveDataLock { get; }

        public string ApprenticeValue =>
            Extract(period.Apprenticeship);

        public string IlrValue =>
            Extract(period.Ilr);

        public bool IsLocked { get; }

        private readonly CollectionPeriod period;
        private readonly Func<DataMatch, object?> valueExtractor;

        public DataLockRowModel(
            CollectionPeriod period,
            DataLock dataLock,
            string heading,
            Func<DataMatch, object?> valueExtractor)
        {
            Heading = heading;
            this.period = period
                ?? throw new ArgumentNullException(nameof(period));
            this.valueExtractor = valueExtractor
                ?? throw new ArgumentNullException(nameof(valueExtractor));
            IsLocked = this.period.DataLocks.Contains(dataLock);
            ActiveDataLock = IsLocked ? dataLock.ToString() : "-";
        }

        public DataLockRowModel(
            CollectionPeriod period,
            string heading,
            Func<DataMatch, object?> value)
            : this(period, 0, heading, value)
        { }

        private string Extract(DataMatch? data) =>
            data != null ? valueExtractor(data)?.ToString() ?? "" : "";
    }
}
