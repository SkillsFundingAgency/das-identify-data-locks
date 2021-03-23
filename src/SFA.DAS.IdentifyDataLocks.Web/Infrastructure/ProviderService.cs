
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Http;

namespace SFA.DAS.IdentifyDataLocks.Web.Infrastructure
{
    public class ProviderService
    {
        private readonly IRoatpService providerApiClient;

        public ProviderService(IRoatpService providerApiClient) =>
            this.providerApiClient = providerApiClient;

        public string GetProviderName(long ukprn)
        {
            try
            {
                var provider = providerApiClient.GetProvider(ukprn);
                return provider.Result.Name;
            }
            catch
            {
                return string.Empty;
            }
        }
    }

    public interface IRoatpService
    {
        Task<Provider> GetProvider(long ukprn);
    }

    public class RoatpService : IRoatpService
    {
        private IRestHttpClient _client;

        public RoatpService(IRoatpApiHttpClientFactory roatpApiHttpClientFactory)
        {
            _client = roatpApiHttpClientFactory.CreateRestHttpClient();
        }

        public async Task<Provider> GetProvider(long ukprn)
        {
            var employerUserEmailQueryUri = $"/api/v1/Search?SearchTerm={ukprn}";

            var providerResult = await _client.Get<RoatpProviderResult>(employerUserEmailQueryUri);

            var provider = providerResult.SearchResults.FirstOrDefault();

            if (provider == null)
            {
                return null;
            }

            return new Provider
            {
                Name = provider.Name,
                Ukprn = provider.Ukprn
            };
        }
    }

    public class Provider
    {
        public int Ukprn { get; set; }
        public string Name { get; set; }
    }

    public class RoatpProviderResult
    {
        [JsonProperty("searchResults")]
        public List<OrganisationSearchResult> SearchResults { get; set; }
    }
    public class OrganisationSearchResult
    {
        [JsonProperty("ukprn")]
        public int Ukprn { get; set; }
        [JsonIgnore]
        public string Name => LegalName ?? TradingName;
        [JsonProperty("tradingName")]
        public string TradingName { get; set; }
        [JsonProperty("legalName")]
        public string LegalName { get; set; }
    }
}