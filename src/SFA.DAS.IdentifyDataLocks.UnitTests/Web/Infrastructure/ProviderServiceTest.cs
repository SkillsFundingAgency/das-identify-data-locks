using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.Http;
using SFA.DAS.IdentifyDataLocks.Domain;
using SFA.DAS.IdentifyDataLocks.Domain.Services;

namespace SFA.DAS.IdentifyDataLocks.UnitTests.Web.Infrastructure
{
    public class ProviderServiceTest
    {
        [Test]
        public async Task WhenValidUkprn_ThenReturnProviderName()
        {
            var mockClient = new Mock<IRestHttpClient>();
            var expectedName = "Provider Name";
            mockClient.Setup(x => x.Get<RoatpProviderResult>(It.IsAny<string>(), null, CancellationToken.None)).ReturnsAsync(new RoatpProviderResult{SearchResults = new List<OrganisationSearchResult>{new OrganisationSearchResult{LegalName = expectedName}}});
            var sut = new ProviderService(mockClient.Object);
            var actual = await sut.GetProvider(1234);
            actual.Should().Be(expectedName);
        }

        [Test]
        public async Task WhenInValidUkprn_ThenReturnEmptyString()
        {
            var mockClient = new Mock<IRestHttpClient>();
            mockClient.Setup(x => x.Get<RoatpProviderResult>(It.IsAny<string>(), null, CancellationToken.None)).Throws(new HttpRequestException());
            var sut = new ProviderService(mockClient.Object);
            var actual = await sut.GetProvider(1234);
            actual.Should().Be(string.Empty);
        }
    }
}