using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using System;
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
                    new { TextContent = "Date: 01/12/2019\n        Price: £24,872.00" },
                    new { TextContent = " - " }
                },
                options =>
                   options.Using<string>(ctx =>
                        ctx.Subject.Trim().Should().Be(ctx.Expectation.Trim()))
                   .WhenTypeIs<string>());

            var tnp2Row = document.QuerySelector("tr.tnp2");
            tnp2Row.Should().NotBeNull();
            tnp2Row.Children.Should().BeEquivalentTo(
                new[]
                {
                    new { TextContent = "TNP 2" },
                    new { TextContent = "n/a" },
                    new { TextContent = "Date: 01/12/2019\n        Price: £1,100.00" },
                    new { TextContent = " - " }
                },
                options =>
                   options.Using<string>(ctx =>
                        ctx.Subject.Trim().Should().Be(ctx.Expectation.Trim()))
                   .WhenTypeIs<string>());
        }

        [Test]
        public async Task RendersTNPsForMultiplePriceEpisodes()
        {
            await Arrange("SFA.DAS.IdentifyDataLocks.IntegrationTests.TestData.LearnerWithMultiplePriceEpisodes");
            TimeProvider.Today.Returns(new DateTime(2020, 08, 01));

            var result = await CreateClient().GetAsync("/Learner/8444744466");

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
                    new { TextContent =
                        "Date: 01/10/2020\n        Price: £4,500.00\n        " +
                        "Date: 07/09/2020\n        Price: £4,500.00" },
                    new { TextContent = " - " }
                },
                options =>
                   options.Using<string>(ctx =>
                        ctx.Subject.Trim().Should().Be(ctx.Expectation.Trim()))
                   .WhenTypeIs<string>());

            var tnp2Row = document.QuerySelector("tr.tnp2");
            tnp2Row.Should().NotBeNull();
            tnp2Row.Children.Should().BeEquivalentTo(
                new[]
                {
                    new { TextContent = "TNP 2" },
                    new { TextContent = "n/a" },
                    new { TextContent =
                        "Date: 01/10/2020\n        Price: £500.00\n        " +
                        "Date: 07/09/2020\n        Price: £0.00" },
                    new { TextContent = " - " }
                },
                options =>
                   options.Using<string>(ctx =>
                        ctx.Subject.Trim().Should().Be(ctx.Expectation.Trim()))
                   .WhenTypeIs<string>());
        }
    }
}