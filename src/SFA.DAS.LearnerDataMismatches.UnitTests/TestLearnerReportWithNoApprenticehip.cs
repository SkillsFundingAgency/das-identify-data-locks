using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.LearnerDataMismatches.Domain;
using System.Linq;

namespace SFA.DAS.LearnerDataMismatches.UnitTests
{
    public class TestLearnerReportWithNoApprenticeship
    {
        [Test]
        public void Finds_datalocks2_when_apprenticeship_is_missing()
        {
            var a = new ApprenticeshipBuilder()
                .ForMissingLearner(uln: 22);

            var sut = a.CreateLearnerReport();

            sut.CollectionPeriods.Should().NotBeEmpty();
            sut.CollectionPeriods.First()
                .Should().BeEquivalentTo(new
                {
                    Apprenticeship = (DataMatch?)null,
                    Ilr = new { Uln = 22 },
                    DataLocks = new [] { DataLock.Dlock02 },
                });
        }
    }
}