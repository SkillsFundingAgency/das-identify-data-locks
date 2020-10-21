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

namespace SFA.DAS.IdentifyDataLocks.IntegrationTests
{
    [Explicit]
    public class LearnerWithMissingApprenticeship
    {
        [SetUp]
        public async Task Arrange()
        {
            await Testing.Reset();

            await Testing.Context.AddEntitiesFromJsonResource<EarningEventModel>("SFA.DAS.IdentifyDataLocks.IntegrationTests.TestData.LearnerWithMissingApprenticeship.EarningEvents.json");

            var dlocks = JsonConvert.DeserializeObject<DataLockEventModel[]>(
                Resources.LoadAsString("SFA.DAS.IdentifyDataLocks.IntegrationTests.TestData.LearnerWithMissingApprenticeship.Dlock02_DataLocks.json"));

            await Testing.Context.AddEntities(dlocks);

            Testing.TimeProvider.Today.Returns(new DateTime(2019, 8, 1));
        }

        [Test]
        public async Task History_only_contains_active_provider()
        {
            var learner = Testing.CreatePage<LearnerModel>();
            learner.Uln = "3123456789";
            await learner.OnGetAsync();

            learner.CurrentYearDataLocks.Should().NotBeEmpty();
        }
    }
}