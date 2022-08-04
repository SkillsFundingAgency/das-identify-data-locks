using System;

namespace SFA.DAS.IdentifyDataLocks.Domain
{
    public struct AcademicYear
    {
        public AcademicYear(DateTime instant)
        {
            var year = instant.Month >= FirstMonthOfAcademicYear ? instant.Year : instant.Year - 1;
            StartingDate = new DateTime(year, FirstMonthOfAcademicYear, 01);
        }

        private AcademicYear(int shortInt)
        {
            var firstYear = shortInt / 100;
            var secondYear = shortInt % 100;

            if (firstYear + 1 != secondYear)
                throw new ArgumentException($"`{shortInt}` does not represent two consecutive years.");

            StartingDate = new DateTime(2000 + firstYear, FirstMonthOfAcademicYear, 01);
        }

        public static implicit operator AcademicYear(int shortInt)
        {
            return new AcademicYear(shortInt);
        }

        public static implicit operator int(AcademicYear year)
        {
            return year.ToShortInt();
        }

        public static AcademicYear operator +(AcademicYear d, int years)
        {
            return new AcademicYear(new DateTime(d.StartingDate.Year + years, d.StartingDate.Month,
                d.StartingDate.Day));
        }

        public static AcademicYear operator -(AcademicYear d, int years)
        {
            return d + -years;
        }

        public override string ToString()
        {
            return $"{StartingDate.Year} - {StartingDate.Year + 1}";
        }

        private int ToShortInt()
        {
            var tens = StartingDate.Year % 100;
            return tens * 100 + tens + 1;
        }

        private const int FirstMonthOfAcademicYear = 8;

        private DateTime StartingDate { get; }
    }
}