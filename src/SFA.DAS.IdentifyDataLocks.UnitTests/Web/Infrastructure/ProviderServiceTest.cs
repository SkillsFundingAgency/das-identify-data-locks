using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.IdentifyDataLocks.Web.Infrastructure;
using SFA.DAS.IdentifyDataLocks.Web.Model;

namespace SFA.DAS.IdentifyDataLocks.UnitTests.Web.Infrastructure
{
    public class ProviderServiceTest
    {
        [Test]
        public async Task WhenValidUkprn_ThenReturnProviderName()
        {
            var mockClient = new Mock<IRoatpService>();
            var expectedName = "Provider Name";
            mockClient.Setup(x => x.GetProvider(It.IsAny<long>())).ReturnsAsync(new Provider { Name = expectedName });
            var sut = new ProviderService(mockClient.Object);
            var actual = await sut.GetProviderName(1234);
            actual.Should().Be(expectedName);
        }

        [Test]
        public async Task WhenInValidUkprn_ThenReturnEmptyString()
        {
            var mockClient = new Mock<IRoatpService>();
            mockClient.Setup(x => x.GetProvider(It.IsAny<long>())).Throws(new HttpRequestException());
            var sut = new ProviderService(mockClient.Object);
            var actual = await sut.GetProviderName(1234);
            actual.Should().Be(string.Empty);
        }
    }
}