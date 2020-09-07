using System;

namespace SFA.DAS.IdentifyDataLocks.Domain
{
    public struct Period : IComparable<Period>, IComparable
    {
        public Period(int year, int month) : this()
        {
            Year = year;
            Month = month;
        }

        public int Year { get; }
        public int Month { get; }

        public override string ToString() =>
            $"R{Month}";

        public int CompareTo(Period other)
            => (Year.CompareTo(other.Year), Month.CompareTo(other.Month))
            switch
            {
                (-1, _) => -1,
                (0, -1) => -1,
                (0, 0) => 0,
                _ => 1,
            };

        public int CompareTo(object obj) =>
            obj is Period other
                ? CompareTo(other)
                : throw new ArgumentException(
                    $"Object is not a {nameof(Period)}");
    }
}
