using System;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.IdentifyDataLocks.Data.Model;
using SFA.DAS.IdentifyDataLocks.IntegrationTests.Helpers;
using SFA.DAS.IdentifyDataLocks.Web.Pages;

namespace SFA.DAS.IdentifyDataLocks.IntegrationTests
{
    public class LearnerWithMissingApprenticeship
    {
        [SetUp]
        public async Task Arrange()
        {
            RazorPagesTestFixture.Reset();

            await RazorPagesTestFixture.Context.AddEntitiesFromJsonResource<EarningEventModel>("SFA.DAS.IdentifyDataLocks.IntegrationTests.TestData.LearnerWithMissingApprenticeship.EarningEvents.json");

            await RazorPagesTestFixture.Context.AddEntitiesFromJsonResource<DataLockEventModel>("SFA.DAS.IdentifyDataLocks.IntegrationTests.TestData.LearnerWithMissingApprenticeship.Dlock02_DataLocks.json");

            RazorPagesTestFixture.TimeProvider.Today.Returns(new DateTime(2019, 8, 1));
        }

        [Test]
        public async Task History_only_contains_active_provider()
        {
            var learner = RazorPagesTestFixture.CreatePage<LearnerModel>();
            learner.Uln = "3123456789";
            await learner.OnGetAsync();

            learner.CurrentYearDataLocks.Should().NotBeEmpty();
        }
    }
}