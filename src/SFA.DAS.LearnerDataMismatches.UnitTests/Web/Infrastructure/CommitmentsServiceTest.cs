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
        [Test]
        public async Task WhenCommitmentExists_ThenReturnApprenticesName()
        {
            var firstName = "firstName";
            var lastName = "lastName";
            var validSearchTerm = "123";
            var apiClientMock = new Mock<ICommitmentsApiClient>();
            var populatedResponse = new GetApprenticeshipsResponse 
            { 
                Apprenticeships = new [] 
                { new GetApprenticeshipsResponse.ApprenticeshipDetailsResponse() {
                    FirstName = firstName, LastName = lastName
                } }
            };
            apiClientMock
                .Setup(a => a
                    .GetApprenticeships(It.Is<GetApprenticeshipsRequest>(x => x.SearchTerm == validSearchTerm), It.IsAny<CancellationToken>())).ReturnsAsync(populatedResponse);
            var sut = new CommitmentsService(apiClientMock.Object);
            var result = await sut.GetApprenticesName(validSearchTerm, 123);

            Assert.AreEqual($"{firstName} {lastName}", result);
        }
        
        [Test]
        public async Task WhenCommitmentDoNotExist_ThenReturnEmptyString()
        {
            var apiClientMock = new Mock<ICommitmentsApiClient>();
            var invalidSearchTerm = "000";

            var emptyResponse = new GetApprenticeshipsResponse { Apprenticeships = Enumerable.Empty<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse>()};
            apiClientMock
                .Setup(a => a
                    .GetApprenticeships(It.Is<GetApprenticeshipsRequest>(x => x.SearchTerm == invalidSearchTerm), It.IsAny<CancellationToken>())).ReturnsAsync(emptyResponse);
            var sut = new CommitmentsService(apiClientMock.Object);
            var result = await sut.GetApprenticesName(invalidSearchTerm, 123);

            Assert.IsEmpty(result);
        }
    }
}