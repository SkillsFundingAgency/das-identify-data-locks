using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Http;
using SFA.DAS.Http.Configuration;

namespace SFA.DAS.IdentifyDataLocks.Domain.Services
{
    public class ProviderService : IProviderService
    {
        private readonly IRestHttpClient _client;

        public ProviderService(IRestHttpClient roatpApiHttpClient)
        {
            _client = roatpApiHttpClient;
        }

        public async Task<string> GetProvider(long ukprn)
        {
            try
            {
                var employerUserEmailQueryUri = $"/api/v1/Search?SearchTerm={ukprn}";

                var providerResult = await _client.Get<RoatpProviderResult>(employerUserEmailQueryUri);

                var provider = providerResult.SearchResults.FirstOrDefault();

                if (provider == null)
                {
                    return null;
                }

                return provider?.Name ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }

    public interface IProviderService
    {
        Task<string> GetProvider(long ukprn);
    }

    public class RoatpApiHttpClientFactory : IRoatpApiHttpClientFactory
    {
        private readonly RoatpApiClientSettings _settings;

        public RoatpApiHttpClientFactory(RoatpApiClientSettings settings)
        {
            _settings = settings;
        }
        public IRestHttpClient CreateClient()
        {
            return new RestHttpClient(new ManagedIdentityHttpClientFactory(_settings).CreateHttpClient());
        }
    }

    public class RoatpApiClientSettings : IManagedIdentityClientConfiguration
    {
        public string ApiBaseUrl { get; set; }
        public string IdentifierUri { get; set; }
    }

    public interface IRoatpApiHttpClientFactory
    {
        IRestHttpClient CreateClient();
    }
}