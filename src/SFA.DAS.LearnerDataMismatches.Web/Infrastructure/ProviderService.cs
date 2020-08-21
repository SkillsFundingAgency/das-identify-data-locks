using System.Net.Http;
using SFA.DAS.Providers.Api.Client;

namespace SFA.DAS.LearnerDataMismatches.Web.Infrastructure
{
    public interface IProviderService
    {
        string GetProviderName(long ukprn);
    }

    public class ProviderService : IProviderService
    {
        private readonly IProviderApiClient _providerApiClient;
        public ProviderService(IProviderApiClient providerApiClient)
        {
            _providerApiClient = providerApiClient;
        }
        public string GetProviderName(long ukprn)
        {
            try
            {
                var provider = _providerApiClient.Get(ukprn);
                return provider.ProviderName;
            }
            catch (HttpRequestException)
            {
                return string.Empty;
            }
        }
    }
}