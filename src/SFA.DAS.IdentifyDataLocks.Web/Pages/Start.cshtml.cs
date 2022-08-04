using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SFA.DAS.IdentifyDataLocks.Web.Pages
{
    public class StartModel : PageModel
    {
        [BindProperty] public string Uln { get; set; } = "";

        public IActionResult OnPost()
        {
            ValidateModel();

            if(ModelState.IsValid)
            {
                return RedirectToPage("learner", new { Uln });
            }

            return Page();
        }

        private void ValidateModel()
        {
            var isUlnANumber = long.TryParse(Uln, out var uln);

            if(isUlnANumber == false || uln <= 0 || uln > 9999999999)
            {
                ModelState.AddModelError(nameof(Uln), "Enter a valid ULN");
            }
        }
    }
}
