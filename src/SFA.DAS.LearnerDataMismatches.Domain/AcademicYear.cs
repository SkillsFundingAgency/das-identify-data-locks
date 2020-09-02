using System;

namespace SFA.DAS.LearnerDataMismatches.Domain
{
    public class AcademicYear
    {
        public int Current { get; }
        public int Previous { get; }

        public string CurrentRange { get; }
        public string PreviousRange { get; }

        public AcademicYear(DateTime today)
        {
            var currentYear = int.Parse(today.ToString("yy"));
            Func<int, int, int> getAcademicYear = (year1, year2) => int.Parse($"{year1}{year2}");
            if(today.Month >= 8)
            {
                Current = getAcademicYear(currentYear, currentYear+1);
                CurrentRange = $"{today.Year} - {today.Year+1}";
                Previous = getAcademicYear(currentYear-1, currentYear);
                PreviousRange = $"{today.Year-1} - {today.Year}";
            }
            else
            {
                Current = getAcademicYear(currentYear-1, currentYear);
                CurrentRange = $"{today.Year-1} - {today.Year}";
                Previous = getAcademicYear(currentYear-2, currentYear-1);
                PreviousRange = $"{today.Year-2} - {today.Year-1}";
            }
        }

        private string GetLastTwoDigits(int value)
        {
            return value.ToString().Substring(2);
        }
    }
}