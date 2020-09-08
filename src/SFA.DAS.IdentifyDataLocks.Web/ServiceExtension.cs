using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.IdentifyDataLocks.Web
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddAuthentication(this IServiceCollection services, AuthenticationConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = WsFederationDefaults.AuthenticationScheme;
                options.DefaultSignOutScheme = WsFederationDefaults.AuthenticationScheme;
            })
            .AddWsFederation(options =>
            {
                options.Wtrealm = configuration.Wtrealm;
                options.MetadataAddress = configuration.MetadataAddress;
                options.UseTokenLifetime = false;
                options.TokenValidationParameters.RoleClaimType = "TEST";
            })
            .AddCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromHours(1);
                options.Cookie.Name = "SFA.DAS.IdentifyDataLocks.Web.Auth";
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.SlidingExpiration = true;
                options.Cookie.SameSite = SameSiteMode.None;
            });

            return services;
        }

        public static IServiceCollection AddAuthorization(this IServiceCollection services, AuthorizationConfiguration configuration)
        {
            services.AddAuthorization(options => 
            {
                options.AddPolicy(AuthorizationConfiguration.PolicyName, policy => 
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim(configuration.ClaimId, configuration.ClaimValue);
                });
            });
            return services;
        }
    }
}