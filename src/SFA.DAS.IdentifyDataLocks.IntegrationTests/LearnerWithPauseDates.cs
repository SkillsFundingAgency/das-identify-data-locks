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
    public class LearnerWithPauseDates
    {
        private ApprenticeshipModel _apprenticeship;

        private async Task Arrange(string apprenticeshipResourceName, bool loadDataLocks = false)
        {
            RazorPagesTestFixture.Reset();

            var apps = await RazorPagesTestFixture.Context.AddEntitiesFromJsonResource<ApprenticeshipModel>(apprenticeshipResourceName);

            _apprenticeship = apps.FirstOrDefault();

            await RazorPagesTestFixture.Context.AddEntitiesFromJsonResource<EarningEventModel>("SFA.DAS.IdentifyDataLocks.IntegrationTests.TestData.ApprenticeshipsWithPausedDates.EarningEvents.json");

            
            if(loadDataLocks)
            {
                await RazorPagesTestFixture.Context.AddEntitiesFromJsonResource<DataLockEventModel>("SFA.DAS.IdentifyDataLocks.IntegrationTests.TestData.ApprenticeshipsWithPausedDates.Dlock12_DataLocks.json");
            }
            else
            {
                await RazorPagesTestFixture.Context.AddEntitiesFromJsonResource<DataLockEventModel>("SFA.DAS.IdentifyDataLocks.IntegrationTests.TestData.ApprenticeshipsWithPausedDates.Dlock06_DataLocks.json");
            }

            RazorPagesTestFixture.TimeProvider.Today.Returns(new DateTime(2019, 08, 01));
        }

        [Test]
        public async Task Active_apprenticeship_with_multiple_pause_record_gets_latest_pause_dates()
        {
            await Arrange("SFA.DAS.IdentifyDataLocks.IntegrationTests.TestData.ApprenticeshipsWithPausedDates.Active_Apprenticeship_With_Multiple_Pause_Dates.json");

            var learner = RazorPagesTestFixture.CreatePage<LearnerModel>();
            learner.Uln = _apprenticeship.Uln.ToString();
            await learner.OnGetAsync();

            learner.CurrentYearDataLocks.First().ApprenticeshipDataMatch.PausedOn.Should().Be(new DateTime(2019, 1, 1));
            learner.CurrentYearDataLocks.First().ApprenticeshipDataMatch.ResumedOn.Should().Be(new DateTime(2019, 3, 1));
            learner.CurrentYearDataLocks.First().ApprenticeshipDataMatch.CompletionStatus.Should().Be(ApprenticeshipStatus.Active);
        }

        [Test]
        public async Task Active_apprenticeship_with_no_pause_record_gets_no_pause_dates()
        {
            await Arrange("SFA.DAS.IdentifyDataLocks.IntegrationTests.TestData.ApprenticeshipsWithPausedDates.Active_Apprenticeship_With_No_Pause_Dates.json");

            var learner = RazorPagesTestFixture.CreatePage<LearnerModel>();
            learner.Uln = _apprenticeship.Uln.ToString();
            await learner.OnGetAsync();

            learner.CurrentYearDataLocks.First().ApprenticeshipDataMatch.PausedOn.Should().BeNull();
            learner.CurrentYearDataLocks.First().ApprenticeshipDataMatch.ResumedOn.Should().BeNull();
            learner.CurrentYearDataLocks.First().ApprenticeshipDataMatch.CompletionStatus.Should().Be(ApprenticeshipStatus.Active);
        }

        [Test]
        public async Task Active_apprenticeship_with_single_pause_record_gets_pause_dates()
        {
            await Arrange("SFA.DAS.IdentifyDataLocks.IntegrationTests.TestData.ApprenticeshipsWithPausedDates.Active_Apprenticeship_With_Single_Pause_Record.json");

            var learner = RazorPagesTestFixture.CreatePage<LearnerModel>();
            learner.Uln = _apprenticeship.Uln.ToString();
            await learner.OnGetAsync();

            learner.CurrentYearDataLocks.First().ApprenticeshipDataMatch.PausedOn.Should().Be(new DateTime(2019, 1, 1));
            learner.CurrentYearDataLocks.First().ApprenticeshipDataMatch.ResumedOn.Should().Be(new DateTime(2019, 3, 1));
            learner.CurrentYearDataLocks.First().ApprenticeshipDataMatch.CompletionStatus.Should().Be(ApprenticeshipStatus.Active);
        }

        [Test]
        public async Task Paused_apprenticeship_gets_latest_pause_dates()
        {
            await Arrange("SFA.DAS.IdentifyDataLocks.IntegrationTests.TestData.ApprenticeshipsWithPausedDates.Paused_Apprenticeship.json", true);

            var learner = RazorPagesTestFixture.CreatePage<LearnerModel>();
            learner.Uln = _apprenticeship.Uln.ToString();
            await learner.OnGetAsync();

            learner.CurrentYearDataLocks.First().ApprenticeshipDataMatch.PausedOn.Should().Be(new DateTime(2018, 6, 7));
            learner.CurrentYearDataLocks.First().ApprenticeshipDataMatch.ResumedOn.Should().BeNull();
            learner.CurrentYearDataLocks.First().ApprenticeshipDataMatch.CompletionStatus.Should().Be(ApprenticeshipStatus.Paused);
        }
    }
}