using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SFA.DAS.LearnerDataMismatches.Pages
{
    public class StartModel : PageModel
    {
        //[BindProperty]
        public string Uln { get; set; }

        public void OnGet()
        {
        }
    }
}
