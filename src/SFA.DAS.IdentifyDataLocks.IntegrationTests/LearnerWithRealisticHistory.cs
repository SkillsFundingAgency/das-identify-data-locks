using FluentAssertions;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SFA.DAS.Apprenticeships.Api.Types.Providers;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.IdentifyDataLocks.Domain;
using SFA.DAS.IdentifyDataLocks.IntegrationTests.Helpers;
using SFA.DAS.IdentifyDataLocks.Web.Pages;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using PaymentsApprenticeshipStatus = SFA.DAS.Payments.Model.Core.Entities.ApprenticeshipStatus;

namespace SFA.DAS.IdentifyDataLocks.IntegrationTests
{
    [Explicit]
    public class LearnerWithRealisticHistory
    {
        [SetUp]
        public async Task SetUp()
        {
            await Testing.Reset();

            var apps = await Testing.Context.AddEntitiesFromJsonResource<ApprenticeshipModel>("SFA.DAS.IdentifyDataLocks.IntegrationTests.TestData.Apprenticeship.json");
            if (apps.Length == 0) throw new Exception("There must be an apprenticeship to run these tests.");

            apprenticeship = apps.FirstOrDefault(x => x.Status == PaymentsApprenticeshipStatus.Active);
            var appid = apprenticeship.Id;

            await Testing.Context.AddEntitiesFromJsonResource<EarningEventModel>("SFA.DAS.IdentifyDataLocks.IntegrationTests.TestData.EarningEvents.json");

            var dlocks = JsonConvert.DeserializeObject<DataLockEventModel[]>(
                Resources.LoadAsString("SFA.DAS.IdentifyDataLocks.IntegrationTests.TestData.DataLocks.json"));
            foreach (var l in dlocks
                .SelectMany(x => x.NonPayablePeriods)
                .SelectMany(x => x.DataLockEventNonPayablePeriodFailures))
            {
                l.ApprenticeshipId = appid;
            }

            await Testing.Context.AddEntities(dlocks);

            Testing.TimeProvider.Today.Returns(new DateTime(2019, 8, 1));
        }

        private ApprenticeshipModel apprenticeship;

        [Test]
        public async Task Finds_collection_period_data()
        {
            var learner = Testing.CreatePage<LearnerModel>();
            learner.Uln = apprenticeship.Uln.ToString();
            await learner.OnGetAsync();

            learner.CurrentYearDataLocks.Should().ContainEquivalentOf(
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
            var learner = Testing.CreatePage<LearnerModel>();
            learner.Uln = apprenticeship.Uln.ToString();
            await learner.OnGetAsync();

            learner.CurrentYearDataLocks.Should()
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
            var learner = Testing.CreatePage<LearnerModel>();
            learner.Uln = apprenticeship.Uln.ToString();
            await learner.OnGetAsync();

            learner.CurrentYearDataLocks.Should()
                .OnlyContain(x => x.Apprenticeship.Ukprn == 10003678);
        }

        [Test]
        public async Task Learner_name_is_shown()
        {
            Testing.CommitmentsApi
                .GetApprenticeships(Arg.Is<GetApprenticeshipsRequest>(
                    req =>
                        req.AccountId == apprenticeship.AccountId &&
                        req.SearchTerm == apprenticeship.Uln.ToString()))
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

            var learner = Testing.CreatePage<LearnerModel>();
            learner.Uln = apprenticeship.Uln.ToString();
            await learner.OnGetAsync();

            learner.LearnerName.Should().Be("LearnerFirstname LearnerLastname");
        }

        [Test]
        public async Task Provider_details_are_shown()
        {
            Testing.ProviderApi
                .Get(apprenticeship.Ukprn)
                .Returns(new Provider
                {
                    Ukprn = apprenticeship.Ukprn,
                    ProviderName = "Best Training Provider",
                });

            var learner = Testing.CreatePage<LearnerModel>();
            learner.Uln = apprenticeship.Uln.ToString();
            await learner.OnGetAsync();

            learner.ProviderId.Should().Be(apprenticeship.Ukprn.ToString());
            learner.ProviderName.Should().Be("Best Training Provider");
        }

        [Test]
        public async Task Account_details_are_shown()
        {
            Testing.AccountsApi
                .GetAccount(apprenticeship.AccountId)
                .Returns(Task.FromResult(new AccountDetailViewModel
                {
                    DasAccountName = "Fantastic Employer",
                    PublicHashedAccountId = "qwerty",
                }));

            var learner = Testing.CreatePage<LearnerModel>();
            learner.Uln = apprenticeship.Uln.ToString();
            await learner.OnGetAsync();

            learner.EmployerId.Should().Be("qwerty");
            learner.EmployerName.Should().Be("Fantastic Employer");
        }

        [Test]
        public async Task Data_locks_are_shown()
        {
            var learner = Testing.CreatePage<LearnerModel>();
            learner.Uln = apprenticeship.Uln.ToString();
            await learner.OnGetAsync();

            learner.DataLockNames.Should().Contain("Dlock01");
        }

        [Test]
        public void Correct_academic_years_are_populated()
        {
            Testing.TimeProvider.Today.Returns(new DateTime(2011, 8, 1));
            var learner = Testing.CreatePage<LearnerModel>();

            learner.AcademicYears.Current.Should().Be((AcademicYear)1112);
            learner.AcademicYears.Previous.Should().Be((AcademicYear)1011);
        }

        [Test]
        public async Task Has_data_locks_should_be_false()
        {
            Testing.TimeProvider.Today.Returns(new DateTime(2011, 8, 1));
            var learner = Testing.CreatePage<LearnerModel>();
            learner.Uln = apprenticeship.Uln.ToString();
            await learner.OnGetAsync();
            learner.HasDataLocks.Should().BeFalse();
        }

        [TestCase(2019, true, false)]
        [TestCase(2020, false, true)]
        public async Task Populate_datalocks_in_right_academic_year_collection(int year, bool expectedHasDataInCurrentYear, bool expectedHasDataInPreviousYear)
        {
            Testing.TimeProvider.Today.Returns(new DateTime(year, 8, 1));
            var learner = Testing.CreatePage<LearnerModel>();
            learner.Uln = apprenticeship.Uln.ToString();
            await learner.OnGetAsync();
            learner.HasDataLocks.Should().BeTrue();
            learner.HasDataLocksInCurrentYear.Should().Be(expectedHasDataInCurrentYear);
            learner.HasDataLocksInPreviousYear.Should().Be(expectedHasDataInPreviousYear);
        }
    }
}