using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.IdentifyDataLocks.Data.Model;
using SFA.DAS.IdentifyDataLocks.Domain;
using SFA.DAS.IdentifyDataLocks.UnitTests.TestBuilders;

namespace SFA.DAS.IdentifyDataLocks.UnitTests
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
                   ApprenticeshipDataMatch = (DataMatch)null,
                   IlrEarningDataMatch = new { Uln = 22 },
                   DataLockErrorCodes = new[] { DataLockErrorCode.Dlock02 },
               });
        }

        [Test]
        public void Finds_datalocks2_when_provider_has_multiple_earnings()
        {
            var a = new ApprenticeshipBuilder()
                .ForProgramme(episodes: e => e.WithPriceFromTnp1And2(10, 10))
                .WithFunctionalSkills();

            var sut = a.CreateLearnerReport();

            sut.CollectionPeriods.First()
               .Should().BeEquivalentTo(new
               {
                   IlrEarningDataMatch = new { Cost = 20 },
               });
        }
    }
}