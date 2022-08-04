using System.Collections.Generic;
using Newtonsoft.Json;

namespace SFA.DAS.IdentifyDataLocks.Domain
{
    public class RoatpProviderResult
    {
        [JsonProperty("searchResults")]
        public List<OrganisationSearchResult> SearchResults { get; set; }
    }
}