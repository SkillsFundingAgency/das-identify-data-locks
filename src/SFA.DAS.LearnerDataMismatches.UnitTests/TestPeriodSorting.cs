using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.LearnerDataMismatches.Domain;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.LearnerDataMismatches.UnitTests
{
    public class TestPeriodSorting
    {
        private static readonly IEnumerable<TestCaseData> values = new[]
        {
            new TestCaseData(
                new[] {new Period(1819, 1), new Period(1819, 2) },
                new[] {new Period(1819, 1), new Period(1819, 2) }),
            new TestCaseData(
                new[] {new Period(1819, 2), new Period(1819, 1) },
                new[] {new Period(1819, 1), new Period(1819, 2) }),
            new TestCaseData(
                new[] {new Period(1819, 1), new Period(1920, 1) },
                new[] {new Period(1819, 1), new Period(1920, 1) }),
            new TestCaseData(
                new[] {new Period(1920, 1), new Period(1819, 1) },
                new[] {new Period(1819, 1), new Period(1920, 1) }),
            new TestCaseData(
                new[] {new Period(1920, 1), new Period(1819, 1) },
                new[] {new Period(1819, 1), new Period(1920, 1) }),
            new TestCaseData(
                new[]
                {
                    new Period(1920, 3),
                    new Period(1920, 1),
                    new Period(1819, 12),
                    new Period(1819, 14),
                    new Period(1819, 13),
                    new Period(1920, 2),
                },
                new[]
                {
                    new Period(1819, 12),
                    new Period(1819, 13),
                    new Period(1819, 14),
                    new Period(1920, 1),
                    new Period(1920, 2),
                    new Period(1920, 3),
                }),
        };

        [Test, TestCaseSource(nameof(values))]
        public void Sorts(Period[] unordered, Period[] expected)
        {
            unordered.OrderBy(x => x).Should().ContainInOrder(expected);
            unordered.OrderBy(x => x).Should().BeInAscendingOrder();
        }
    }
}