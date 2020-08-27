using FluentAssertions;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.LearnerDataMismatches.Domain;
using SFA.DAS.LearnerDataMismatches.Web.Pages;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.LearnerDataMismatches.IntegrationTests
{
    public class LearnerWithNoHistory
    {
        [SetUp]
        public async Task SetUp()
        {
            await Testing.Reset();

            var apps = await Testing.AddEntitiesFromJsonResource<ApprenticeshipModel>("SFA.DAS.LearnerDataMismatches.IntegrationTests.TestData.Apprenticeship.json");
            var appid = apps.FirstOrDefault()?.Id;
            LearnerUln = apps.FirstOrDefault()?.Uln.ToString();

            await Testing.AddEntitiesFromJsonResource<EarningEventModel>("SFA.DAS.LearnerDataMismatches.IntegrationTests.TestData.EarningEvents.json");

            var dlocks = JsonConvert.DeserializeObject<DataLockEventModel[]>(
                Resources.LoadAsString("SFA.DAS.LearnerDataMismatches.IntegrationTests.TestData.DataLocks.json"));
            foreach (var l in dlocks
                .SelectMany(x => x.NonPayablePeriods)
                .SelectMany(x => x.DataLockEventNonPayablePeriodFailures))
            {
                l.ApprenticeshipId = appid;
            }

            await Testing.AddEntities(dlocks);
        }

        private string LearnerUln;

        [Test]
        public async Task Finds_collection_period_data()
        {
            var learner = Testing.Create<LearnerModel>();
            learner.Uln = LearnerUln;
            await learner.OnGetAsync();

            learner.NewCollectionPeriods.Should().ContainEquivalentOf(
                new
                {
                    Apprenticeship = new
                    {
                        Ukprn = 10003678,
                        Standard = 50,
                        Framework = 0,
                        Program = 25,
                        Pathway = 0,
                        Cost = 25972.0,
                        PriceStart = new DateTime(2019, 12, 01),
                        CompletionStatus = Domain.ApprenticeshipStatus.Active,
                    },
                    Ilr = new
                    {
                        Ukprn = 10003678,
                        Standard = 50,
                        Framework = 0,
                        Program = 25,
                        Pathway = 0,
                        Cost = 25972.0,
                        PriceStart = new DateTime(2019, 12, 01),
                        //CompletionStatus = Domain.ApprenticeshipStatus.Active,
                    }
                });
        }

        [Test]
        public async Task History_is_ordered()
        {
            var learner = Testing.Create<LearnerModel>();
            learner.Uln = LearnerUln;
            await learner.OnGetAsync();

            learner.NewCollectionPeriods.Should()
                .NotBeEmpty()
                .And.BeInDescendingOrder()
                .And.BeEquivalentTo(
                    new { Period = new Period(1920, 12) },
                    new { Period = new Period(1920, 11) },
                    new { Period = new Period(1920, 10) },
                    new { Period = new Period(1920, 9) },
                    new { Period = new Period(1920, 8) },
                    new { Period = new Period(1920, 7) },
                    new { Period = new Period(1920, 6) },
                    new { Period = new Period(1920, 5) }
                                   );
        }

        [Test]
        public async Task History_only_contains_active_provider()
        {
            var learner = Testing.Create<LearnerModel>();
            learner.Uln = LearnerUln;
            await learner.OnGetAsync();

            learner.NewCollectionPeriods.Should()
                .OnlyContain(x => x.Apprenticeship.Ukprn == 10003678);
        }

        [Test]
        public async Task Learner_name_is_found()
        {
            Testing.CommitmentsApi
                .GetApprenticeships(Arg.Is<GetApprenticeshipsRequest>(req =>
                    req.AccountId == 15084 && req.SearchTerm == "2839925663"))
                .Returns(Task.FromResult(new GetApprenticeshipsResponse
                {
                    Apprenticeships = new[]
                    {
                        new GetApprenticeshipsResponse.ApprenticeshipDetailsResponse
                        {
                            FirstName = "LearnerFirstname",
                            LastName = "LearnerLastname",
                        }
                    }
                }));

            var learner = Testing.Create<LearnerModel>();
            learner.Uln = LearnerUln;
            await learner.OnGetAsync();

            learner.LearnerName.Should().Be("LearnerFirstname LearnerLastname");
        }

        [Test]
        public async Task Data_locks_are_shown()
        {
            var learner = Testing.Create<LearnerModel>();
            learner.Uln = LearnerUln;
            await learner.OnGetAsync();

            learner.DataLockNames.Should().Contain("Dlock01");
        }
    }
}