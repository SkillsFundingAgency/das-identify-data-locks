namespace SFA.DAS.IdentifyDataLocks.Web
{
    public class AuthorizationConfiguration
    {
        public const string PolicyName = "StaffPolicy";
        public string ClaimId { get; set; }
        public string ClaimValue { get; set; }
    }
}