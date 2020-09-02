using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.LearnerDataMismatches.Domain;

namespace SFA.DAS.LearnerDataMismatches.UnitTests.Domain
{
    public class AcademicYearTests
    {
        static object[] TestData = 
        {
            new object[] {new DateTime(2019,8,1), 1920, 1819},
            new object[] {new DateTime(2019,7,31), 1819, 1718}
        };

        [TestCaseSource("TestData")]
        public void GenerateCorrectAcademicYears(DateTime today, int expectedCurrentYear, int expectedPreviousYear)
        {
            var model = new AcademicYear(today);

            model.Current.Should().Be(expectedCurrentYear);
            model.Previous.Should().Be(expectedPreviousYear);
        }
    }
}