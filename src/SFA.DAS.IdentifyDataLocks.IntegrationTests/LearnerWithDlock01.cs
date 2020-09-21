using FluentAssertions;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SFA.DAS.IdentifyDataLocks.IntegrationTests.Helpers;
using SFA.DAS.IdentifyDataLocks.Web.Pages;
using SFA.DAS.IdentifyDataLocks.Domain;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using PaymentsApprenticeshipStatus = SFA.DAS.Payments.Model.Core.Entities.ApprenticeshipStatus;

namespace SFA.DAS.IdentifyDataLocks.IntegrationTests
{
    [Explicit]
    public class LearnerWithDlock01
    {
        [SetUp]
        public async Task SetUp()
        {
            await Testing.Reset();

            var apps = await Testing.Context.AddEntitiesFromJsonResource<ApprenticeshipModel>("SFA.DAS.IdentifyDataLocks.IntegrationTests.TestData.Dlock01_Apprenticeship.json");
            if (apps.Length == 0) throw new Exception("There must be an apprenticeship to run these tests.");

            apprenticeship = apps.FirstOrDefault(x => x.Status == PaymentsApprenticeshipStatus.Active);
            var appid = apprenticeship.Id;

            await Testing.Context.AddEntitiesFromJsonResource<EarningEventModel>("SFA.DAS.IdentifyDataLocks.IntegrationTests.TestData.Dlock01_Earnings.json");

            var dlocks = JsonConvert.DeserializeObject<DataLockEventModel[]>(
                Resources.LoadAsString("SFA.DAS.IdentifyDataLocks.IntegrationTests.TestData.Dlock01_DataLocks.json"));
            foreach (var l in dlocks
                .SelectMany(x => x.NonPayablePeriods)
                .SelectMany(x => x.DataLockEventNonPayablePeriodFailures))
            {
                l.ApprenticeshipId = appid;
            }

            await Testing.Context.AddEntities(dlocks);
        }

        private ApprenticeshipModel apprenticeship;

        [Test]
        public async Task Searching_for_apprenticeship_UKPRN_finds_dlocks()
        {
            Testing.TimeProvider.Today.Returns(new DateTime(2020, 08, 01));

            var learner = Testing.CreatePage<LearnerModel>();
            learner.Uln = apprenticeship.Uln.ToString();
            await learner.OnGetAsync();

            learner.CurrentYearDataLocks.Should()
                .Contain(x => x.Apprenticeship.Ukprn == 10004000);

            learner.CurrentYearDataLocks.Should()
                .ContainEquivalentOf(new
                {
                    Apprenticeship = new { Ukprn = 10004000 },
                    Ilr = new { Ukprn = 10001000 },
                    DataLocks = new[]
                    {
                        DataLock.Dlock01
                    },
                });
        }
    }
}