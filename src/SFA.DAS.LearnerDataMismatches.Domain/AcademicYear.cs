using System;

namespace SFA.DAS.LearnerDataMismatches.Domain
{
    public struct AcademicYear
    {
        public DateTime StartingDate { get; }
        public int ShortRepresentation { get; }

        public override string ToString() =>
            $"{StartingDate.Year} - {StartingDate.Year + 1}";

        public AcademicYear PreviousAcademicYear => this - 1;
        public AcademicYear(DateTime instant)
        {
            var year = instant.Month >= 8 ? instant.Year : instant.Year - 1;
            StartingDate = new DateTime(year, 08, 01);
            var tens = year % 100;
            ShortRepresentation = ((tens * 100) + tens + 1);
        }

        public static AcademicYear operator +(AcademicYear d, int years)
            => new AcademicYear(
                new DateTime(
                    d.StartingDate.Year + years,
                    d.StartingDate.Month,
                    d.StartingDate.Day));
        public static AcademicYear operator -(AcademicYear d, int years)
            => d + (-years);
    }
}