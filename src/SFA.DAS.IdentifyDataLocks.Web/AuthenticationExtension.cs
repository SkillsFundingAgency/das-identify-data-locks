using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.IdentifyDataLocks.Web
{
    public static class AuthenticationExtension
    {
        public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
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
                options.Wtrealm = configuration["Wtrealm"];
                options.MetadataAddress = configuration["MetaDataAddress"];
                options.UseTokenLifetime = false;
                options.TokenValidationParameters.RoleClaimType = "TEST";
            })
            .AddCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromHours(1);
                options.Cookie.Name = "SFA.DAS.LearnerDataMismatches.Web.Auth";
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.SlidingExpiration = true;
                options.Cookie.SameSite = SameSiteMode.None;
            });

            return services;
        }
    }
}