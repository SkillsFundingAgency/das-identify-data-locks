using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;

namespace SFA.DAS.LearnerDataMismatches.Web.Infrastructure
{
    public interface ICommitmentsService
    {
        Task<string> GetApprenticesName(string uln, long accountId);
    }

    public class CommitmentsService : ICommitmentsService
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        public CommitmentsService(ICommitmentsApiClient commitmentsApiClient)
        {
            _commitmentsApiClient = commitmentsApiClient;
        }

        public async Task<string> GetApprenticesName(string uln, long accountId)
        {
            var request = new GetApprenticeshipsRequest()
            {
                AccountId = accountId,
                SearchTerm = uln
            };
            var result = await _commitmentsApiClient.GetApprenticeships(request);
            var apprenticeship = result.Apprenticeships.FirstOrDefault();
            if (apprenticeship == null) return string.Empty;
            return $"{apprenticeship.FirstName} {apprenticeship.LastName}";
        }
    }
}