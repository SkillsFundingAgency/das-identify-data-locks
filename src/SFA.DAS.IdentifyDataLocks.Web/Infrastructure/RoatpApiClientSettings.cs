﻿using SFA.DAS.Http.Configuration;

namespace SFA.DAS.IdentifyDataLocks.Web.Infrastructure
{
    public class RoatpApiClientSettings : IManagedIdentityClientConfiguration
    {
            public string ApiBaseUrl { get; set; }
            public string IdentifierUri { get; set; }

    }

}