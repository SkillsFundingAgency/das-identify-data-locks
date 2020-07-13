using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LearnerDataMismatches.Pages
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
