using System;
using SFA.DAS.IdentifyDataLocks.Domain;

namespace SFA.DAS.IdentifyDataLocks.Web.Pages
{
    public class PauseDateDataLockRowModel
    {
        private readonly Func<DataMatch, (DateTime? pausedOn, DateTime? resumedOn)> dataFunc;
        public string PausedOnDate { get; }
        public string ResumedOnDate { get; }
        public bool HasPausedDate { get; }
        public string Heading { get; }
        public string ActiveDataLock { get; }
        public bool IsLocked { get; }

        public PauseDateDataLockRowModel(
            CollectionPeriod period,
            DataLock dataLock,
            string heading,
            Func<DataMatch, (DateTime? pausedOn, DateTime? resumedOn)> dataFunc) 
        {
            Heading = heading;
            IsLocked = period.DataLocks.Contains(dataLock);
            ActiveDataLock = IsLocked ? dataLock.ToString() : "-";
            (DateTime? pausedOnDate, DateTime? resumedOnDate) = dataFunc.Invoke(period.Apprenticeship);
            HasPausedDate = pausedOnDate.HasValue;
            PausedOnDate = pausedOnDate?.ToShortDateString();
            ResumedOnDate = resumedOnDate.HasValue ? resumedOnDate.GetValueOrDefault().ToShortDateString() : "Present";
        }
    }
}