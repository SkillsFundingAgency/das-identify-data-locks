using Microsoft.Extensions.Configuration;
using SFA.DAS.DfESignIn.Auth.Enums;
using SFA.DAS.DfESignIn.Auth.Interfaces;
using SFA.DAS.IdentifyDataLocks.Web.Constants;

namespace SFA.DAS.IdentifyDataLocks.Web.Infrastructure
{
    /// <summary>
    /// Class responsible for attaching the CustomServiceRole post DfESignIn process.
    /// </summary>
    public class CustomServiceRole : ICustomServiceRole
    {
        private readonly AuthorizationConfiguration _authorizationConfiguration;

        public CustomServiceRole(IConfiguration configuration)
        {
            _authorizationConfiguration = configuration.GetSection(ConfigKey.Authorization).Get<AuthorizationConfiguration>();
        }
        public string RoleClaimType => _authorizationConfiguration.ClaimId;
        public CustomServiceRoleValueType RoleValueType => CustomServiceRoleValueType.Name;
    }
}
