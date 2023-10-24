using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SFA.DAS.IdentifyDataLocks.Web.Pages
{
    [Authorize(Policy = AuthorizationConfiguration.PolicyName)]
    public class StartModel : PageModel
    {
        [BindProperty]
        public string Uln { get; set; }

        public async Task<IActionResult> OnPost()
        {
            await ValidateModel();
            if(ModelState.IsValid)
            {
                return RedirectToPage("learner", new { Uln = Uln });
            }
            else
            {
                return Page();
            }
        }

        private async Task ValidateModel()
        {
            var isUlnANumber = long.TryParse(Uln, out long uln);
            if(Uln == null || isUlnANumber == false || uln <= 0 || uln > 9999999999)
            {
                ModelState.AddModelError(nameof(Uln), "Enter a valid ULN");
            }
        }
    }
}
