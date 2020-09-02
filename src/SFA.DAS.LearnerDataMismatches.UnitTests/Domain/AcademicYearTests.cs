using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.LearnerDataMismatches.Domain;

namespace SFA.DAS.LearnerDataMismatches.UnitTests.Domain
{
    public class AcademicYearTests
    {
        static object[] AcademicYearTestData = 
        {
            new object[] {new DateTime(2019,8,1), 1920, 1819},
            new object[] {new DateTime(2019,7,31), 1819, 1718}
        };

        [TestCaseSource("AcademicYearTestData")]
        public void GenerateCorrectAcademicYears(DateTime today, int expectedCurrentYear, int expectedPreviousYear)
        {
            var model = new AcademicYear(today);

            model.Current.Should().Be(expectedCurrentYear);
            model.Previous.Should().Be(expectedPreviousYear);
        }
        static object[] AcademicYearRangeTestData = 
        {
            new object[] {new DateTime(2019,8,1), "2019 - 2020", "2018 - 2019"},
            new object[] {new DateTime(2019,7,31), "2018 - 2019", "2017 - 2018"}
        };

        [TestCaseSource("AcademicYearRangeTestData")]
        public void GenerateCorrectAcademicYearRanges(DateTime today, string expectedCurrentYearRange, string expectedPreviousYearRange)
        {
            var model = new AcademicYear(today);

            model.CurrentRange.Should().Be(expectedCurrentYearRange);
            model.PreviousRange.Should().Be(expectedPreviousYearRange);
        }
    }
}