using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SFA.DAS.IdentifyDataLocks.Web.Constants;
using System;

namespace SFA.DAS.IdentifyDataLocks.Web.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class AccessDenied : PageModel
    {
        private readonly IConfiguration _configuration;
        private string _integrationUrlPart = string.Empty;
        private string _environment = string.Empty;

        public bool UseDfESignIn { get; set; }

        public AccessDenied(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet()
        {
            UseDfESignIn = _configuration.GetValue<bool>(ConfigKey.UseDfESignIn);
            _environment = _configuration.GetValue<string>(ConfigKey.ResourceEnvironmentName);
        }
        

        /// <summary>
        /// Gets DfESignIn Select service link.
        /// </summary>
        public string HelpPageLink
        {
            get
            {
                if (!string.IsNullOrEmpty(_environment) && !_environment.Equals("prd", StringComparison.CurrentCultureIgnoreCase))
                {
                    _integrationUrlPart = "test-";
                }
                return $"https://{_integrationUrlPart}services.signin.education.gov.uk/approvals/select-organisation?action=request-service";
            }
        }
    }
}
