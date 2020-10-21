using FluentAssertions;
using NUnit.Framework;
using System.Threading.Tasks;

namespace SFA.DAS.IdentifyDataLocks.IntegrationTests.LearnerWithStoppedApprenticeship
{
    [Explicit]
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

    [Explicit]
    public class LearnerWithMultiplePriceEpisodes : WebApplicationTestFixture
    {
        [Test]
        public async Task RendersTNPs()
        {
            await Arrange("SFA.DAS.IdentifyDataLocks.IntegrationTests.TestData");

            var result = await CreateClient().GetAsync("/Learner/2839925663");

            result.EnsureSuccessStatusCode(); // Status Code 200-299
            result.Content.Should().NotBeNull();
            var body = await result.Content.ReadAsStringAsync();
            body.Should().Contain("&#xA3;24,872.00");
        }
    }
}