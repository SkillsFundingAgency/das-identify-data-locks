namespace SFA.DAS.IdentifyDataLocks.Web.Infrastructure
{
    public class RedisConnectionSettings
    {
        public string RedisConnectionString { get; set; }
        public string DataProtectionKeysDatabase { get; set; }
    }
}
