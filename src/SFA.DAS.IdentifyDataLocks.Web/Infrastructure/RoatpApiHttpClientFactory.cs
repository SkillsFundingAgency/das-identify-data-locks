using SFA.DAS.Http;

namespace SFA.DAS.IdentifyDataLocks.Web.Infrastructure
{
    public class RoatpApiHttpClientFactory : IRoatpApiHttpClientFactory
    {
        private readonly RoatpApiClientSettings _settings;

        public RoatpApiHttpClientFactory(RoatpApiClientSettings settings)
        {
            _settings = settings;
        }
        public IRestHttpClient CreateRestHttpClient()
        {
            return new RestHttpClient(new ManagedIdentityHttpClientFactory(_settings).CreateHttpClient());
        }
    }

    public interface IRoatpApiHttpClientFactory
    {
        IRestHttpClient CreateRestHttpClient();
    }
}