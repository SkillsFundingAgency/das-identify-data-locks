using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.IdentifyDataLocks.Domain;

namespace SFA.DAS.IdentifyDataLocks.UnitTests.Domain
{
    public class AcademicYearTests
    {
        private static readonly object[] AcademicYearTestData =
        {
            new object[] {new DateTime(2019,8,1), 1920, 1819},
            new object[] {new DateTime(2019,7,31), 1819, 1718}
        };

        [TestCaseSource(nameof(AcademicYearTestData))]
        public void GenerateCorrectAcademicYears(DateTime today, int expectedCurrentYear, int expectedPreviousYear)
        {
            var model = new AcademicYear(today);

            model.Should().Be((AcademicYear)expectedCurrentYear);
            (model - 1).Should().Be((AcademicYear)expectedPreviousYear);
        }

        [TestCaseSource(nameof(AcademicYearTestData))]
        public void GenerateAcademicYearsFromShortInt(DateTime instant, int shortInt, int _)
        {
            var fromShortInt = (AcademicYear)shortInt;
            var fromDateTime = new AcademicYear(instant);
            fromShortInt.Should().Be(fromDateTime);
        }

        [Test]
        public void RefusesToGenerateAcademicYearsFromInvalidShortInt()
        {
            var act = () => { AcademicYear _ = 1821; };
            act
              .Should().Throw<ArgumentException>()
              .And.Message.Should().Contain("1821", because: "Exceptions should report the data that caused them");
        }

        private static readonly object[] AcademicYearRangeTestData =
        {
            new object[] {new DateTime(2019,8,1), "2019 - 2020", "2018 - 2019"},
            new object[] {new DateTime(2019,7,31), "2018 - 2019", "2017 - 2018"}
        };

        [TestCaseSource(nameof(AcademicYearRangeTestData))]
        public void GenerateCorrectAcademicYearRanges(DateTime today, string expectedCurrentYearRange, string expectedPreviousYearRange)
        {
            var model = new AcademicYear(today);

            model.ToString().Should().Be(expectedCurrentYearRange);
            (model - 1).ToString().Should().Be(expectedPreviousYearRange);
        }
    }
}