using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.IdentifyDataLocks.Web.Pages;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;
using PaymentsApprenticeshipStatus = SFA.DAS.Payments.Model.Core.Entities.ApprenticeshipStatus;

namespace SFA.DAS.IdentifyDataLocks.IntegrationTests
{
    [Explicit]
    public class LearnerWithMultipleProviders
    {
        private ApprenticeshipModel apprenticeship;
        
        [SetUp]
        public async Task SetUp()
        {
            await Testing.Reset();

            var apps = await Testing.Context.AddEntitiesFromJsonResource<ApprenticeshipModel>("SFA.DAS.IdentifyDataLocks.IntegrationTests.TestData.LearnerWithMultipleProviders.Apprenticeship.json");
            if (apps.Length == 0) throw new Exception("There must be an apprenticeship to run these tests.");

            apprenticeship = apps.FirstOrDefault(x => x.Status == PaymentsApprenticeshipStatus.Active);

            await Testing.Context.AddEntitiesFromJsonResource<EarningEventModel>("SFA.DAS.IdentifyDataLocks.IntegrationTests.TestData.LearnerWithMultipleProviders.EarningEvents.json");
        }

        [Test]
        public async Task Searching_for_apprenticeship_finds_multiple_providers()
        {
            Testing.TimeProvider.Today.Returns(new DateTime(2019, 08, 01));

            var learner = Testing.CreatePage<LearnerModel>();
            learner.Uln = apprenticeship.Uln.ToString();
            await learner.OnGetAsync();

            learner.HasMultipleProviders.Should().BeTrue();
        }
    }
}