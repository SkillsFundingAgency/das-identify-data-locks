using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.IdentifyDataLocks.Web.Infrastructure;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.IdentifyDataLocks.Web.Pages
{
    public class PriceEpisodeModel
    {
        public string Identifier { get; set; }
    }

    public class LearnerDetailsModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public long Uln { get; set; } = 9000000407;

        public IEnumerable<ApprenticeshipModel> Apprenticeship { get; set; }
        public IEnumerable<DataLockEventModel> DataLocks { get; set; }
        public IEnumerable<PriceEpisodeModel> PriceEpisodes { get; set; }
        public IEnumerable<IGrouping<long, EarningEventModel>> Earnings { get; set; }

        private readonly IArchivedPaymentsDataContext context;
        private readonly DataLockService dataLockService;

        public LearnerDetailsModel(IArchivedPaymentsDataContext context, DataLockService dataLockService)
        {
            this.context = context;
            this.dataLockService = dataLockService;
        }


        public async Task OnGet()
        {
            Apprenticeship = await context
                .Apprenticeship
                .Include(x => x.ApprenticeshipPriceEpisodes)
                .Where(x => x.Uln == Uln)
                .ToListAsync();

            DataLocks = await dataLockService.GetDataLocks(Uln);

            Earnings = (await context
                .EarningEvent
                .Include(x => x.PriceEpisodes)
                .Where(x => x.LearnerUln == Uln)
                .OrderByDescending(x => x.Ukprn)
                .OrderByDescending(x => x.AcademicYear)
                .ThenByDescending(x => x.CollectionPeriod)
                .ToListAsync())
                .GroupBy(x => x.Ukprn)
                ;
        }
    }

}
