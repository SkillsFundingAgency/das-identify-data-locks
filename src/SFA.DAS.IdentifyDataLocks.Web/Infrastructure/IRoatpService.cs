using System.Threading.Tasks;
using SFA.DAS.Http;
using SFA.DAS.IdentifyDataLocks.Web.Model;

namespace SFA.DAS.IdentifyDataLocks.Web.Infrastructure
{
    public interface IRoatpService
    {
        Task<Provider?> GetProvider(long ukprn);
    }

    public class RoatpService : IRoatpService
    {
        private readonly IRestHttpClient _client;

        public RoatpService(IRoatpApiHttpClientFactory roatpApiHttpClientFactory)
        {
            _client = roatpApiHttpClientFactory.CreateClient();
        }

        public async Task<Provider?> GetProvider(long ukprn)
        {
            var getOrganisationDetailsPath = $"/organisations/{ukprn}";

            try
            {
                var provider = await _client.Get<OrganisationSearchResult>(getOrganisationDetailsPath);

                return new Provider
                {
                    Name = provider.Name,
                    Ukprn = provider.Ukprn
                };

            }
            catch
            {
                return null;
            }

        }
    }

}