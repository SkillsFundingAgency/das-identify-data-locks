using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LearnerDataMismatches.Pages
{
    public class LearnerModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Uln { get; set; } = "9000000407";

        public void OnGet()
        {
        }
    }
}
