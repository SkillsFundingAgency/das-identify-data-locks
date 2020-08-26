using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Repositories;

namespace SFA.DAS.LearnerDataMismatches.Web.Pages
{
    public class StartModel : PageModel
    {
        private readonly IPaymentsDataContext _context;
        [BindProperty]
        public string Uln { get; set; }

        public StartModel(IPaymentsDataContext context) =>
            _context = context;

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
            else
            {
                var apprenticeship = await _context.Apprenticeship.FirstOrDefaultAsync(x => x.Uln == uln);

                if (apprenticeship == null)
                {
                    ModelState.AddModelError(nameof(Uln), "ULN not found");
                }
            }

        }
    }
}
