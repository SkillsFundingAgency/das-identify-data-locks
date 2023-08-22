using System.Linq;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.IdentifyDataLocks.IntegrationTests.Helpers;
using SFA.DAS.Payments.Model.Core.Entities;
using PaymentsApprenticeshipStatus = SFA.DAS.Payments.Model.Core.Entities.ApprenticeshipStatus;
using SFA.DAS.Payments.Model.Core.Audit;
using NSubstitute;
using SFA.DAS.IdentifyDataLocks.Web.Pages;
using FluentAssertions;

namespace SFA.DAS.IdentifyDataLocks.IntegrationTests
{
    [Explicit]
    public class LearnerWithArchivedAndCurrentDataLocks
    {
        private ApprenticeshipModel apprenticeship;

        [SetUp]
        public async Task SetUp()
        {
            await Testing.Reset();

            var apps = await Testing.Context.AddEntitiesFromJsonResource<ApprenticeshipModel>("SFA.DAS.IdentifyDataLocks.IntegrationTests.TestData.LearnerWithArchivedAndCurrentDataLocks.Apprenticeship.json");
            if (apps.Length == 0) throw new Exception("There must be an apprenticeship to run these tests.");

            apprenticeship = apps.FirstOrDefault(x => x.Status == PaymentsApprenticeshipStatus.Active);
            var appid = apprenticeship.Id;

            await Testing.Context.AddEntitiesFromJsonResource<EarningEventModel>("SFA.DAS.IdentifyDataLocks.IntegrationTests.TestData.LearnerWithArchivedAndCurrentDataLocks.EarningEvents_Archive.json");
            await Testing.Context.AddEntitiesFromJsonResource<EarningEventModel>("SFA.DAS.IdentifyDataLocks.IntegrationTests.TestData.LearnerWithArchivedAndCurrentDataLocks.EarningEvents_Current.json");
            
            var archiveDataLocks = JsonConvert.DeserializeObject<DataLockEventModel[]>(
                Resources.LoadAsString("SFA.DAS.IdentifyDataLocks.IntegrationTests.TestData.LearnerWithArchivedAndCurrentDataLocks.Datalocks_Archive.json"));
            foreach (var l in archiveDataLocks
                         .SelectMany(x => x.NonPayablePeriods)
                         .SelectMany(x => x.DataLockEventNonPayablePeriodFailures))
            {
                l.ApprenticeshipId = appid;
            }

            var currentPeriodDataLocks = JsonConvert.DeserializeObject<DataLockEventModel[]>(
                Resources.LoadAsString("SFA.DAS.IdentifyDataLocks.IntegrationTests.TestData.LearnerWithArchivedAndCurrentDataLocks.Datalocks_Current.json"));
            foreach (var l in currentPeriodDataLocks
                         .SelectMany(x => x.NonPayablePeriods)
                         .SelectMany(x => x.DataLockEventNonPayablePeriodFailures))
            {
                l.ApprenticeshipId = appid;
            }

            await Testing.Context.AddEntities(currentPeriod: false, archiveDataLocks);
            await Testing.Context.AddEntities(currentPeriod: true, currentPeriodDataLocks);
        }

        [Test]
        public async Task Archived_data_locks_are_included()
        {
            Testing.TimeProvider.Today.Returns(new DateTime(2020, 08, 01));

            var learner = Testing.CreatePage<LearnerModel>();
            learner.Uln = apprenticeship.Uln.ToString();
            await learner.OnGetAsync();

            learner.PreviousYearDataLocks.Should()
                .Contain(x => x.Apprenticeship.Ukprn == 10000610);
        }

        [Test]
        public async Task Current_period_data_locks_are_included()
        {
            Testing.TimeProvider.Today.Returns(new DateTime(2020, 08, 01));

            var learner = Testing.CreatePage<LearnerModel>();
            learner.Uln = apprenticeship.Uln.ToString();
            await learner.OnGetAsync();

            learner.CurrentYearDataLocks.Should()
                .Contain(x => x.Apprenticeship.Ukprn == 10000610);
        }
    }
}
