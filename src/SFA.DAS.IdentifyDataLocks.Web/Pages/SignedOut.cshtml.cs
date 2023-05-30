using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using SFA.DAS.IdentifyDataLocks.Web.Constants;

namespace SFA.DAS.IdentifyDataLocks.Web.Pages
{
    public class SignedOutModel : PageModel
    {
        private readonly IConfiguration _config;

        public SignedOutModel(IConfiguration config)
        {
            _config = config;
        }

        public IActionResult OnGet()
        {
            // condition to check if DfESignIn is allowed.
            var useDfESignIn = _config.GetSection(ConfigKey.UseDfESignIn).Get<bool>();
            var authScheme = useDfESignIn
                ? OpenIdConnectDefaults.AuthenticationScheme
                : WsFederationDefaults.AuthenticationScheme;

            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }

            return SignOut(
                new AuthenticationProperties { RedirectUri = Url.Page("SignedOut", null, null, Request.Scheme) },
                CookieAuthenticationDefaults.AuthenticationScheme,
                authScheme);
        }
    }
}