using Newtonsoft.Json;

namespace SFA.DAS.IdentifyDataLocks.Domain
{
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