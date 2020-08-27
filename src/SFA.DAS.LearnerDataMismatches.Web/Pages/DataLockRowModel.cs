using SFA.DAS.LearnerDataMismatches.Domain;
using System;

namespace SFA.DAS.LearnerDataMismatches.Web.Pages
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
        private readonly Func<DataMatch, object> valueExtractor;

        public DataLockRowModel(
            string heading,
            DataLock data,
            CollectionPeriod period,
            Func<DataMatch, object> value)
        {
            Heading = heading;
            this.period = period;
            valueExtractor = value;
            IsLocked = this.period.DataLocks.Contains(data);
            ActiveDataLock = IsLocked ? data.ToString() : "-";
        }
    }
}