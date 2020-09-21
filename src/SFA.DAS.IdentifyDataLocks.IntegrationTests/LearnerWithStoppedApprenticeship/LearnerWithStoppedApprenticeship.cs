using FluentAssertions;
using NUnit.Framework;
using System.Threading.Tasks;

namespace SFA.DAS.IdentifyDataLocks.IntegrationTests.LearnerWithStoppedApprenticeship
{
    public class LearnerWithStoppedApprenticeship : WebApplicationTestFixture
    {
        [Test]
        public async Task RendersDataMismatches()
        {
            await Arrange("SFA.DAS.IdentifyDataLocks.IntegrationTests.TestData.LearnerWithStoppedApprenticeship");

            var result = await CreateClient().GetAsync("/Learner/6279465119");

            result.EnsureSuccessStatusCode(); // Status Code 200-299
            result.Content.Headers.ContentType.ToString().Should().Be("text/html; charset=utf-8");
        }
    }
}