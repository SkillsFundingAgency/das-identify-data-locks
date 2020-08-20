using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.LearnerDataMismatches.Web.Infrastructure;

namespace SFA.DAS.LearnerDataMismatches.UnitTests.Infrastructure
{
    public class CommitmentsServiceTest
    {
        private Mock<ICommitmentsApiClient> _apiClientMock = new Mock<ICommitmentsApiClient>();
        private CommitmentsService _sut;

        private const string _validSearchTerm = "123";
        private const string _invalidSearchTerm = "000";
        private const long _accountId = 1111;
        private const string _firstName = "firstName";
        private const string _lastName = "lastName";

        private GetApprenticeshipsResponse _emptyResponse = new GetApprenticeshipsResponse { Apprenticeships = Enumerable.Empty<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse>()};
        private GetApprenticeshipsResponse _populatedResponse = new GetApprenticeshipsResponse 
        { 
            Apprenticeships = new [] 
            { new GetApprenticeshipsResponse.ApprenticeshipDetailsResponse() {
                FirstName = _firstName, LastName = _lastName
            } }
        };

        [SetUp]
        public void InitialiseTest()
        {
            _apiClientMock
                .Setup(a => a
                    .GetApprenticeships(It.Is<GetApprenticeshipsRequest>(x => x.SearchTerm == _validSearchTerm), It.IsAny<CancellationToken>())).ReturnsAsync(_populatedResponse);
            _apiClientMock
                .Setup(a => a
                    .GetApprenticeships(It.Is<GetApprenticeshipsRequest>(x => x.SearchTerm == _invalidSearchTerm), It.IsAny<CancellationToken>())).ReturnsAsync(_emptyResponse);

            _sut = new CommitmentsService(_apiClientMock.Object);
        }

        [Test]
        public async Task WhenCommitmentExists_ThenReturnApprenticesName()
        {
            var result = await _sut.GetApprenticesName(_validSearchTerm, _accountId);

            Assert.AreEqual($"{_firstName} {_lastName}", result);
        }
        
        [Test]
        public async Task WhenCommitmentDoNotExist_ThenReturnEmptyString()
        {
            var result = await _sut.GetApprenticesName(_invalidSearchTerm, _accountId);

            Assert.IsEmpty(result);
        }
    }
}