using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.DfESignIn.Auth.AppStart;
using SFA.DAS.IdentifyDataLocks.Web.Constants;
using SFA.DAS.IdentifyDataLocks.Web.Infrastructure;

namespace SFA.DAS.IdentifyDataLocks.Web
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration config)
        {
            // condition to check if DfESignIn is allowed.
            var useDfESignIn = config.GetSection(ConfigKey.UseDfESignIn).Get<bool>();
            if (useDfESignIn)
            {
                // register DfeSignIn authentication services to the AspNetCore Authentication Options.
                services.AddAndConfigureDfESignInAuthentication(
                    config,
                    CookieName.AuthCookie,
                    typeof(CustomServiceRole),
                    DfESignIn.Auth.Enums.ClientName.DataLocks,
                    "/SignedOut",
                    "");
            }
            else
            {
                var authenticationConfig = config.GetSection(ConfigKey.Authentication).Get<AuthenticationConfiguration>();
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = WsFederationDefaults.AuthenticationScheme;
                    options.DefaultSignOutScheme = WsFederationDefaults.AuthenticationScheme;
                })
                .AddWsFederation(options =>
                {
                    options.Wtrealm = authenticationConfig.Wtrealm;
                    options.MetadataAddress = authenticationConfig.MetadataAddress;
                    options.UseTokenLifetime = false;
                    options.TokenValidationParameters.RoleClaimType = "TEST";
                })
                .AddCookie(options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromHours(1);
                    options.Cookie.Name = CookieName.AuthCookie;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.SlidingExpiration = true;
                    options.Cookie.SameSite = SameSiteMode.None;
                });
            }

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