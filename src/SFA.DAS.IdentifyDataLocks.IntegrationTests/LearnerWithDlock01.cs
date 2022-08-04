using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.IdentifyDataLocks.Data.Model;
using SFA.DAS.IdentifyDataLocks.IntegrationTests.Helpers;
using SFA.DAS.IdentifyDataLocks.Web.Pages;

namespace SFA.DAS.IdentifyDataLocks.IntegrationTests
{
    public class LearnerWithDlock01
    {
        [SetUp]
        public async Task SetUp()
        {
            RazorPagesTestFixture.Reset();

            var apps = await RazorPagesTestFixture.Context.AddEntitiesFromJsonResource<ApprenticeshipModel>("SFA.DAS.IdentifyDataLocks.IntegrationTests.TestData.Dlock01_Apprenticeship.json");
            if (apps.Length == 0) throw new Exception("There must be an apprenticeship to run these tests.");

            _apprenticeship = apps.FirstOrDefault(x => x.Status == ApprenticeshipStatus.Active);

            await RazorPagesTestFixture.Context.AddEntitiesFromJsonResource<EarningEventModel>("SFA.DAS.IdentifyDataLocks.IntegrationTests.TestData.Dlock01_Earnings.json");

            await RazorPagesTestFixture.Context.AddEntitiesFromJsonResource<DataLockEventModel>("SFA.DAS.IdentifyDataLocks.IntegrationTests.TestData.Dlock01_DataLocks.json");
        }

        private ApprenticeshipModel _apprenticeship;

        [Test]
        public async Task Searching_for_apprenticeship_UKPRN_finds_dlocks()
        {
            RazorPagesTestFixture.TimeProvider.Today.Returns(new DateTime(2020, 08, 01));

            var learner = RazorPagesTestFixture.CreatePage<LearnerModel>();
            learner.Uln = _apprenticeship.Uln.ToString();
            await learner.OnGetAsync();

            learner.CurrentYearDataLocks.Should().Contain(x => x.ApprenticeshipDataMatch.Ukprn == 10004000);

            learner.CurrentYearDataLocks.Should().ContainEquivalentOf(new
            {
                ApprenticeshipDataMatch = new { Ukprn = 10004000 },
                IlrEarningDataMatch = new { Ukprn = 10001000 },
                DataLockErrorCodes = new[]
                {
                    DataLockErrorCode.Dlock01
                },
            });
        }
    }
}