using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.IdentifyDataLocks.IntegrationTests.Helpers;
using SFA.DAS.IdentifyDataLocks.Web.Pages;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.IdentifyDataLocks.IntegrationTests
{
    public class LearnerWithPauseDates
    {
        private ApprenticeshipModel apprenticeship;

        public async Task Arrange(string apprenticeshipResourceName, bool loadDataLocks = false)
        {
            await Testing.Reset();

            var apps = await Testing.Context.AddEntitiesFromJsonResource<ApprenticeshipModel>(apprenticeshipResourceName);

            apprenticeship = apps.FirstOrDefault();

            await Testing.Context.AddEntitiesFromJsonResource<EarningEventModel>("SFA.DAS.IdentifyDataLocks.IntegrationTests.TestData.ApprenticeshipsWithPausedDates.EarningEvents.json");

            
            if(loadDataLocks)
            {
                await LoadDataLocksFromJson("SFA.DAS.IdentifyDataLocks.IntegrationTests.TestData.ApprenticeshipsWithPausedDates.Dlock12_DataLocks.json");
            }
            else
            {
                await LoadDataLocksFromJson("SFA.DAS.IdentifyDataLocks.IntegrationTests.TestData.ApprenticeshipsWithPausedDates.Dlock06_DataLocks.json");
            }

            Testing.TimeProvider.Today.Returns(new DateTime(2019, 08, 01));
        }

        private async Task LoadDataLocksFromJson(string filename)
        {
            var dataLocks = JsonConvert.DeserializeObject<DataLockEventModel[]>(
                Resources.LoadAsString(filename));
            foreach (var l in dataLocks
            .SelectMany(x => x.NonPayablePeriods)
            .SelectMany(x => x.DataLockEventNonPayablePeriodFailures))
            {
                l.ApprenticeshipId = apprenticeship.Id;
            }
            await Testing.Context.AddEntities(dataLocks);
        }

        [Test]
        public async Task Active_apprenticeship_with_multiple_pause_record_gets_latest_pause_dates()
        {
            await Arrange("SFA.DAS.IdentifyDataLocks.IntegrationTests.TestData.ApprenticeshipsWithPausedDates.Active_Apprenticeship_With_Multiple_Pause_Dates.json");

            var learner = Testing.CreatePage<LearnerModel>();
            learner.Uln = apprenticeship.Uln.ToString();
            await learner.OnGetAsync();

            learner.CurrentYearDataLocks.First().Apprenticeship.PausedOn.Should().Be(new DateTime(2019, 1, 1));
            learner.CurrentYearDataLocks.First().Apprenticeship.ResumedOn.Should().Be(new DateTime(2019, 3, 1));
            learner.CurrentYearDataLocks.First().Apprenticeship.CompletionStatus.Should().Be(ApprenticeshipStatus.Active);
        }

        [Test]
        public async Task Active_apprenticeship_with_no_pause_record_gets_no_pause_dates()
        {
            await Arrange("SFA.DAS.IdentifyDataLocks.IntegrationTests.TestData.ApprenticeshipsWithPausedDates.Active_Apprenticeship_With_No_Pause_Dates.json");

            var learner = Testing.CreatePage<LearnerModel>();
            learner.Uln = apprenticeship.Uln.ToString();
            await learner.OnGetAsync();

            learner.CurrentYearDataLocks.First().Apprenticeship.PausedOn.Should().BeNull();
            learner.CurrentYearDataLocks.First().Apprenticeship.ResumedOn.Should().BeNull();
            learner.CurrentYearDataLocks.First().Apprenticeship.CompletionStatus.Should().Be(ApprenticeshipStatus.Active);
        }

        [Test]
        public async Task Active_apprenticeship_with_single_pause_record_gets_pause_dates()
        {
            await Arrange("SFA.DAS.IdentifyDataLocks.IntegrationTests.TestData.ApprenticeshipsWithPausedDates.Active_Apprenticeship_With_Single_Pause_Record.json");

            var learner = Testing.CreatePage<LearnerModel>();
            learner.Uln = apprenticeship.Uln.ToString();
            await learner.OnGetAsync();

            learner.CurrentYearDataLocks.First().Apprenticeship.PausedOn.Should().Be(new DateTime(2019, 1, 1));
            learner.CurrentYearDataLocks.First().Apprenticeship.ResumedOn.Should().Be(new DateTime(2019, 3, 1));
            learner.CurrentYearDataLocks.First().Apprenticeship.CompletionStatus.Should().Be(ApprenticeshipStatus.Active);
        }

        [Test]
        public async Task Paused_apprenticeship_gets_latest_pause_dates()
        {
            await Arrange("SFA.DAS.IdentifyDataLocks.IntegrationTests.TestData.ApprenticeshipsWithPausedDates.Paused_Apprenticeship.json", true);

            var learner = Testing.CreatePage<LearnerModel>();
            learner.Uln = apprenticeship.Uln.ToString();
            await learner.OnGetAsync();

            learner.CurrentYearDataLocks.First().Apprenticeship.PausedOn.Should().Be(new DateTime(2018, 6, 7));
            learner.CurrentYearDataLocks.First().Apprenticeship.ResumedOn.Should().BeNull();
            learner.CurrentYearDataLocks.First().Apprenticeship.CompletionStatus.Should().Be(ApprenticeshipStatus.Paused);
        }
    }
}