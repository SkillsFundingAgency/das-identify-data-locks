using SFA.DAS.EAS.Account.Api.Client;
using System.Threading.Tasks;

namespace SFA.DAS.IdentifyDataLocks.Web.Infrastructure
{
    public class EmployerService
    {
        private readonly IAccountApiClient accountApiClient;

        public EmployerService(IAccountApiClient accountApiClient) =>
            this.accountApiClient = accountApiClient;

        public async Task<(string employerName, string publicAccountId)> GetEmployerName(long accountId)
        {
            try
            {
                var result = await accountApiClient.GetAccount(accountId);
                return (result.DasAccountName, result.PublicHashedAccountId);
            }
            catch
            {
                //if the employer id is invalid the service throws 500
                return (string.Empty, string.Empty);
            }
        }
    }
}