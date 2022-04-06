using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.IdentifyDataLocks.Web.Infrastructure;
using StackExchange.Redis;

namespace SFA.DAS.IdentifyDataLocks.Web.Extensions
{
    public static class DataProtectionStartupExtensions
    {
        public static IServiceCollection AddDataProtection(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            if (!environment.IsDevelopment())
            {
                var config = configuration.GetSection(nameof(RedisConnectionSettings)).Get<RedisConnectionSettings>();

                if (config != null)
                {
                    var redisConnectionString = config.RedisConnectionString;
                    var dataProtectionKeysDatabase = config.DataProtectionKeysDatabase;

                    var redis = ConnectionMultiplexer
                        .Connect($"{redisConnectionString},{dataProtectionKeysDatabase}");

                    services.AddDataProtection()
                        .SetApplicationName("das-identify-data-locks-web")
                        .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");
                }
            }

            return services;
        }
    }
}
