using System;

namespace SFA.DAS.LearnerDataMismatches.Domain
{
    public struct Period : IComparable<Period>
    {
        public Period(int year, int month) : this()
        {
            Year = year;
            Month = month;
        }

        public int Year { get; }
        public int Month { get; }

        public override string ToString() =>
            $"{Year}-R{Month}";

        public int CompareTo(Period other)
            => (Year.CompareTo(other.Year), Month.CompareTo(other.Month))
            switch
            {
                (-1, _) => -1,
                (0, -1) => -1,
                (0, 0) => 0,
                _ => 1,
            };
    }
}
