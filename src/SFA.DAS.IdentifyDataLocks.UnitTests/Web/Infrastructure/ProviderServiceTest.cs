using System.Net.Http;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Api.Types.Providers;
using SFA.DAS.IdentifyDataLocks.Web.Infrastructure;
using SFA.DAS.Providers.Api.Client;

namespace SFA.DAS.IdentifyDataLocks.UnitTests.Web.Infrastructure
{
    public class ProviderServiceTest
    {
        [Test]
        public void WhenValidUkprn_ThenReturnProviderName()
        {
            var mockClient = new Mock<IProviderApiClient>();
            var expectedName = "Provider Name";
            mockClient.Setup(x => x.Get(It.IsAny<long>())).Returns(new Provider { ProviderName = expectedName });
            var sut = new ProviderService(mockClient.Object);
            var actual = sut.GetProviderName(1234);
            actual.Should().Be(expectedName);
        }

        [Test]
        public void WhenInValidUkprn_ThenReturnEmptyString()
        {
            var mockClient = new Mock<IProviderApiClient>();
            mockClient.Setup(x => x.Get(It.IsAny<long>())).Throws(new HttpRequestException());
            var sut = new ProviderService(mockClient.Object);
            var actual = sut.GetProviderName(1234);
            actual.Should().BeEmpty();
        }
    }
}