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
        public async Task RendersTNPForOncePriceEpisodes()
        {
            await Arrange("SFA.DAS.IdentifyDataLocks.IntegrationTests.TestData");

            var result = await CreateClient().GetAsync("/Learner/2839925663");

            result.EnsureSuccessStatusCode(); // Status Code 200-299
            result.Content.Should().NotBeNull();
            var body = await result.Content.ReadAsStringAsync();

            var parser = new AngleSharp.Html.Parser.HtmlParser();
            var document = parser.ParseDocument(body);
            var tnp1Row = document.QuerySelector("tr.tnp1");
            tnp1Row.Should().NotBeNull();
            tnp1Row.Children.Should().BeEquivalentTo(
                new[]
                {
                    new { TextContent = "TNP 1" },
                    new { TextContent = "n/a" },
                    new { TextContent = "Date: 01/12/2019\n        Price: �24,872.00" },
                    new { TextContent = " - " }
                },
                options =>
                   options.Using<string>(ctx =>
                        ctx.Subject.Trim().Should().Be(ctx.Expectation.Trim()))
                   .WhenTypeIs<string>());
        }
    }
}