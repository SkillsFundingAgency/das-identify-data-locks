using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.IdentifyDataLocks.Data.Model;
using SFA.DAS.IdentifyDataLocks.Domain;
using SFA.DAS.IdentifyDataLocks.IntegrationTests.Helpers;
using SFA.DAS.IdentifyDataLocks.Web.Pages;

namespace SFA.DAS.IdentifyDataLocks.IntegrationTests
{
    public class LearnerWithRealisticHistory
    {
        [SetUp]
        public async Task SetUp()
        {
            RazorPagesTestFixture.Reset();

            var apps = await RazorPagesTestFixture.Context.AddEntitiesFromJsonResource<ApprenticeshipModel>("SFA.DAS.IdentifyDataLocks.IntegrationTests.TestData.Apprenticeships.json");
            if (apps.Length == 0) throw new Exception("There must be an apprenticeship to run these tests.");

            _apprenticeship = apps.FirstOrDefault(x => x.Status == ApprenticeshipStatus.Active);

            await RazorPagesTestFixture.Context.AddEntitiesFromJsonResource<EarningEventModel>("SFA.DAS.IdentifyDataLocks.IntegrationTests.TestData.EarningEvents.json");

            await RazorPagesTestFixture.Context.AddEntitiesFromJsonResource<DataLockEventModel>("SFA.DAS.IdentifyDataLocks.IntegrationTests.TestData.Datalocks.json");

            RazorPagesTestFixture.TimeProvider.Today.Returns(new DateTime(2019, 8, 1));
        }

        private ApprenticeshipModel _apprenticeship;

        [Test]
        public async Task Finds_collection_period_data()
        {
            var learner = RazorPagesTestFixture.CreatePage<LearnerModel>();
            learner.Uln = _apprenticeship.Uln.ToString();
            await learner.OnGetAsync();

            learner.CurrentYearDataLocks.Should().ContainEquivalentOf(
                new
                {
                    ApprenticeshipDataMatch = new
                    {
                        Ukprn = 10003678,
                        Standard = 50,
                        Framework = 0,
                        Program = 25,
                        Pathway = 0,
                        Cost = 25972.0,
                        PriceStart = new DateTime(2019, 12, 01),
                        CompletionStatus = ApprenticeshipStatus.Active,
                    },
                    IlrEarningDataMatch = new
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
            var learner = RazorPagesTestFixture.CreatePage<LearnerModel>();
            learner.Uln = _apprenticeship.Uln.ToString();
            await learner.OnGetAsync();

            var collectionPeriods = new List<CollectionPeriod>
            {
               new CollectionPeriod { Period = new Period(1920, 12) },
               new CollectionPeriod { Period = new Period(1920, 11) },
               new CollectionPeriod { Period = new Period(1920, 10) },
               new CollectionPeriod { Period = new Period(1920, 9) },
               new CollectionPeriod { Period = new Period(1920, 8) },
               new CollectionPeriod { Period = new Period(1920, 7) },
               new CollectionPeriod { Period = new Period(1920, 6) },
               new CollectionPeriod { Period = new Period(1920, 5) }
            };

            learner.CurrentYearDataLocks.Should()
                .NotBeEmpty()
                .And.BeInDescendingOrder()
                .And.BeEquivalentTo(collectionPeriods, option => option.Excluding(x=> x.ApprenticeshipDataMatch).Excluding(x => x.DataLockErrorCodes).Excluding(x => x.IlrEarningDataMatch));
        }

        [Test]
        public async Task History_only_contains_active_provider()
        {
            var learner = RazorPagesTestFixture.CreatePage<LearnerModel>();
            learner.Uln = _apprenticeship.Uln.ToString();
            await learner.OnGetAsync();

            learner.CurrentYearDataLocks.Should()
                .OnlyContain(x => x.ApprenticeshipDataMatch.Ukprn == 10003678);
        }

        [Test]
        public async Task Learner_name_is_shown()
        {
            RazorPagesTestFixture.CommitmentsApi
                .GetApprenticeships(Arg.Is<GetApprenticeshipsRequest>(
                    req =>
                        req.AccountId == _apprenticeship.AccountId &&
                        req.SearchTerm == _apprenticeship.Uln.ToString()))
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

            var learner = RazorPagesTestFixture.CreatePage<LearnerModel>();
            learner.Uln = _apprenticeship.Uln.ToString();
            await learner.OnGetAsync();

            learner.LearnerName.Should().Be("LearnerFirstname LearnerLastname");
        }

        [Test]
        public async Task Provider_details_are_shown()
        {
            RazorPagesTestFixture.ProviderApi
                .GetProvider(_apprenticeship.Ukprn)
                .Returns(info =>  "Best Training Provider");

            var learner = RazorPagesTestFixture.CreatePage<LearnerModel>();
            learner.Uln = _apprenticeship.Uln.ToString();
            await learner.OnGetAsync();

            learner.ProviderId.Should().Be(_apprenticeship.Ukprn.ToString());
            learner.ProviderName.Should().Be("Best Training Provider");
        }

        [Test]
        public async Task Account_details_are_shown()
        {
            RazorPagesTestFixture.AccountsApi
                .GetAccount(_apprenticeship.AccountId)
                .Returns(Task.FromResult(new AccountDetailViewModel
                {
                    DasAccountName = "Fantastic Employer",
                    PublicHashedAccountId = "qwerty",
                }));

            var learner = RazorPagesTestFixture.CreatePage<LearnerModel>();
            learner.Uln = _apprenticeship.Uln.ToString();
            await learner.OnGetAsync();

            learner.EmployerId.Should().Be("qwerty");
            learner.EmployerName.Should().Be("Fantastic Employer");
        }

        [Test]
        public async Task Data_locks_are_shown()
        {
            var learner = RazorPagesTestFixture.CreatePage<LearnerModel>();
            learner.Uln = _apprenticeship.Uln.ToString();
            await learner.OnGetAsync();

            learner.DataLockNames.Should().Contain("Dlock01");
        }

        [Test]
        public void Correct_academic_years_are_populated()
        {
            RazorPagesTestFixture.TimeProvider.Today.Returns(new DateTime(2011, 8, 1));
            var learner = RazorPagesTestFixture.CreatePage<LearnerModel>();

            learner.AcademicYears.Current.Should().Be((AcademicYear)1112);
            learner.AcademicYears.Previous.Should().Be((AcademicYear)1011);
        }

        [Test]
        public async Task Has_data_locks_should_be_false()
        {
            RazorPagesTestFixture.TimeProvider.Today.Returns(new DateTime(2011, 8, 1));
            var learner = RazorPagesTestFixture.CreatePage<LearnerModel>();
            learner.Uln = _apprenticeship.Uln.ToString();
            await learner.OnGetAsync();
            learner.HasDataLocks.Should().BeFalse();
        }

        [TestCase(2019, true, false)]
        [TestCase(2020, false, true)]
        public async Task Populate_datalocks_in_right_academic_year_collection(int year, bool expectedHasDataInCurrentYear, bool expectedHasDataInPreviousYear)
        {
            RazorPagesTestFixture.TimeProvider.Today.Returns(new DateTime(year, 8, 1));
            var learner = RazorPagesTestFixture.CreatePage<LearnerModel>();
            learner.Uln = _apprenticeship.Uln.ToString();
            await learner.OnGetAsync();
            learner.HasDataLocks.Should().BeTrue();
            learner.HasDataLocksInCurrentYear.Should().Be(expectedHasDataInCurrentYear);
            learner.HasDataLocksInPreviousYear.Should().Be(expectedHasDataInPreviousYear);
        }
    }
}