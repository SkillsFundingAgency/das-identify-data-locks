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
    public class LearnerWithMultipleProviders
    {
        private ApprenticeshipModel _apprenticeship;
        
        [SetUp]
        public async Task SetUp()
        {
            RazorPagesTestFixture.Reset();

            var apps = await RazorPagesTestFixture.Context.AddEntitiesFromJsonResource<ApprenticeshipModel>("SFA.DAS.IdentifyDataLocks.IntegrationTests.TestData.LearnerWithMultipleProviders.Apprenticeship.json");
            if (apps.Length == 0) throw new Exception("There must be an apprenticeship to run these tests.");

            _apprenticeship = apps.FirstOrDefault(x => x.Status == ApprenticeshipStatus.Active);

            await RazorPagesTestFixture.Context.AddEntitiesFromJsonResource<EarningEventModel>("SFA.DAS.IdentifyDataLocks.IntegrationTests.TestData.LearnerWithMultipleProviders.EarningEvents.json");
        }

        [Test]
        public async Task Searching_for_apprenticeship_finds_multiple_providers()
        {
            RazorPagesTestFixture.TimeProvider.Today.Returns(new DateTime(2019, 08, 01));

            var learner = RazorPagesTestFixture.CreatePage<LearnerModel>();
            learner.Uln = _apprenticeship.Uln.ToString();
            await learner.OnGetAsync();

            learner.HasMultipleProviders.Should().BeTrue();
        }
    }
}