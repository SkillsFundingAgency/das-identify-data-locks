using SFA.DAS.Providers.Api.Client;

namespace SFA.DAS.IdentifyDataLocks.Web.Infrastructure
{
    public class ProviderService
    {
        private readonly IProviderApiClient providerApiClient;

        public ProviderService(IProviderApiClient providerApiClient) =>
            this.providerApiClient = providerApiClient;

        public string GetProviderName(long ukprn)
        {
            try
            {
                var provider = providerApiClient.Get(ukprn);
                return provider.ProviderName;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}