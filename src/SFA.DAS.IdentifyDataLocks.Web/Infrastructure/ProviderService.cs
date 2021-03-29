using System.Threading.Tasks;

namespace SFA.DAS.IdentifyDataLocks.Web.Infrastructure
{
    public class ProviderService
    {
        private readonly IRoatpService providerApiClient;

        public ProviderService(IRoatpService providerApiClient) =>
            this.providerApiClient = providerApiClient;

        public async Task<string> GetProviderName(long ukprn)
        {
            try
            {
                var provider = await providerApiClient.GetProvider(ukprn);
                return provider.Name ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}