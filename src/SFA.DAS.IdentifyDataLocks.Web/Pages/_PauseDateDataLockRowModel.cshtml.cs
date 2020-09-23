using SFA.DAS.IdentifyDataLocks.Domain;

namespace SFA.DAS.IdentifyDataLocks.Web.Pages
{
    public class PauseDateDataLockRowModel
    {
        public string? PausedOnDate { get; }
        public string? ResumedOnDate { get; }
        public bool HasPausedDate => PausedOnDate != null;
        public string Heading { get; }
        public string ActiveDataLock { get; }
        public bool IsLocked { get; }

        public PauseDateDataLockRowModel(
            CollectionPeriod period,
            DataLock dataLock,
            string heading)
        {
            Heading = heading;
            IsLocked = period.DataLocks.Contains(dataLock);
            ActiveDataLock = IsLocked ? dataLock.ToString() : "-";
            PausedOnDate = period.Apprenticeship?.PausedOn?.ToShortDateString();
            ResumedOnDate = period.Apprenticeship?.ResumedOn?.ToShortDateString() ?? "Present";
        }
    }
}