using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.LearnerDataMismatches.Web.TagHelpers;

namespace SFA.DAS.LearnerDataMismatches.Web.Pages
{
    public class StartModel : PageModel
    {
        [BindProperty]
        public string Uln { get; set; }

        public IActionResult OnPost()
        {
            ValidateModel();
            if(ModelState.IsValid)
            {
                return RedirectToPage("learner");
            }
            else
            {
                return Page();
            }
        }

        private void ValidateModel()
        {
            var isUlnANumber = long.TryParse(Uln, out long uln);
            if(Uln == null || isUlnANumber == false || uln <= 0 || uln > 9999999999)
            {
                ModelState.AddModelError(nameof(Uln), "Enter a valid ULN");
            }
        }
    }
}
