using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Account.Api.Client;

namespace SFA.DAS.LearnerDataMismatches.Web.Infrastructure
{
    public interface IEmployerService
    {
        Task<(string employerName, string publicAccountId)> GetEmployerName(long accountId);
    }

    public class EmployerService : IEmployerService
    {
        private readonly IAccountApiClient _accountApiClient;
        public EmployerService(IAccountApiClient accountApiClient)
        {
            _accountApiClient = accountApiClient;
        }

        public async Task<(string employerName, string publicAccountId)> GetEmployerName(long accountId)
        {
            try
            {
                var result = await _accountApiClient.GetAccount(accountId);
                return (result.DasAccountName, result.PublicHashedAccountId);
            }
            catch (Exception)
            {
                //if the employer id is invalid the service throws 500
                return (string.Empty, string.Empty);
            }
        }
    }
}