using System.Net.Http;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.IdentifyDataLocks.Web.Infrastructure;

namespace SFA.DAS.IdentifyDataLocks.UnitTests.Web.Infrastructure
{
    public class ProviderServiceTest
    {
        [Test]
        public void WhenValidUkprn_ThenReturnProviderName()
        {
            var mockClient = new Mock<IRoatpService>();
            var expectedName = "Provider Name";
            mockClient.Setup(x => x.GetProvider(It.IsAny<long>())).ReturnsAsync(new Provider { Name = expectedName });
            var sut = new ProviderService(mockClient.Object);
            var actual = sut.GetProviderName(1234);
            actual.Should().Be(expectedName);
        }

        [Test]
        public void WhenInValidUkprn_ThenReturnEmptyString()
        {
            var mockClient = new Mock<IRoatpService>();
            mockClient.Setup(x => x.GetProvider(It.IsAny<long>())).Throws(new HttpRequestException());
            var sut = new ProviderService(mockClient.Object);
            var actual = sut.GetProviderName(1234);
            actual.Should().BeEmpty();
        }
    }
}