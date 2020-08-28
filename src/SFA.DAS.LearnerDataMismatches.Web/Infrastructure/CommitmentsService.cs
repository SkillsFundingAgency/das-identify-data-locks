using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.LearnerDataMismatches.Web.Infrastructure
{
    public class CommitmentsService
    {
        private readonly ICommitmentsApiClient commitmentsApiClient;

        public CommitmentsService(ICommitmentsApiClient commitmentsApiClient) =>
            this.commitmentsApiClient = commitmentsApiClient;

        public async Task<string> GetApprenticesName(string uln, long accountId)
        {
            try
            {
                var request = new GetApprenticeshipsRequest()
                {
                    AccountId = accountId,
                    SearchTerm = uln
                };
                var result = await commitmentsApiClient.GetApprenticeships(request);
                var apprenticeship = result?.Apprenticeships?.FirstOrDefault();
                if (apprenticeship == null) return string.Empty;
                return $"{apprenticeship.FirstName} {apprenticeship.LastName}";
            }
            catch
            {
                return "";
            }
        }
    }
}