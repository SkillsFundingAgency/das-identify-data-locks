using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.IdentifyDataLocks.Web.Infrastructure;

namespace SFA.DAS.IdentifyDataLocks.UnitTests.Web.Infrastructure
{
    public class EmployerServiceTest
    {
        private readonly Mock<IAccountApiClient> _mockAccountApiClient = new Mock<IAccountApiClient>();

        [Test]
        public async Task WhenInvalidEmployerAccountId_ThenReturnEmptyString()
        {
            _mockAccountApiClient.Setup(x => x.GetAccount(It.IsAny<long>())).Throws(new Exception());
            var sut = new EmployerService(_mockAccountApiClient.Object);
            var result = await sut.GetEmployerName(123);
            result.Should().Be((string.Empty, string.Empty));
        }

        [Test]
        public async Task WhenInvalidEmployerAccountId_ThenReturnEmployerName()
        {
            var expectedName = "employer name";
            var expectedAccountId = "expectedid";
            _mockAccountApiClient.Setup(x => x.GetAccount(It.IsAny<long>())).ReturnsAsync(new AccountDetailViewModel { DasAccountName = expectedName, PublicHashedAccountId = expectedAccountId });
            var sut = new EmployerService(_mockAccountApiClient.Object);
            var result = await sut.GetEmployerName(123);
            result.Should().Be((expectedName, expectedAccountId));
        }
    }
}