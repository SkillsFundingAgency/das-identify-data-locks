using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Http;
using SFA.DAS.IdentifyDataLocks.Web.Model;

namespace SFA.DAS.IdentifyDataLocks.Web.Infrastructure
{
    public interface IRoatpService
    {
        Task<Provider> GetProvider(long ukprn);
    }

    public class RoatpService : IRoatpService
    {
        private IRestHttpClient _client;

        public RoatpService(IRoatpApiHttpClientFactory roatpApiHttpClientFactory)
        {
            _client = roatpApiHttpClientFactory.CreateClient();
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

}