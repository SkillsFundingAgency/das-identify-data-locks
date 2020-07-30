using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SFA.DAS.LearnerDataMismatches.Web.Pages
{
    public class StartModel : PageModel
    {
        [BindProperty]
        public int? Uln { get; set; }

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
            if(Uln == null || Uln <= 0 || Uln > 9999999999)
            {
                ModelState.AddModelError(nameof(Uln), "Enter a valid ULN");
            }
        }
    }
}
