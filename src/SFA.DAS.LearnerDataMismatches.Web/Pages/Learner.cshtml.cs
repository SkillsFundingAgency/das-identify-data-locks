using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.LearnerDataMismatches.Domain;
using SFA.DAS.LearnerDataMismatches.Web.Infrastructure;

namespace SFA.DAS.LearnerDataMismatches.Web.Pages 
{
    public class LearnerModel : PageModel 
    {
        [BindProperty (SupportsGet = true)]
        public string Uln { get; set; }

        public IEnumerable<Domain.CollectionPeriod> NewCollectionPeriods { get; private set; } = Enumerable.Empty<Domain.CollectionPeriod> ();

        public string LearnerName { get; set; }

        public string EmployerName { get; set; }
        public string EmployerId { get; set; }
        public string ProviderName { get; set; }
        public string ProviderId { get; set; }
        public IEnumerable<string> DataLockNames =>
            NewCollectionPeriods
            .SelectMany (x => x.DataLocks)
            .Select (x => x.ToString ())
            .OrderBy (x => x);

        public IEnumerable<DataLockHelpCentreLink> DataLockLinks =>
            NewCollectionPeriods
            .SelectMany (x => x.DataLocks)
            .Distinct ()
            .Select (DataLockHelpCentreLink.Create)
            .OrderBy (x => x.Name);
        
        public AcademicYear AcademicYear => new AcademicYear(DateTime.Today);
        private readonly LearnerReportProvider learnerReportProvider;

        public LearnerModel (LearnerReportProvider learnerReportProvider) 
        {
            this.learnerReportProvider = learnerReportProvider;
        }

        public async Task OnGetAsync () 
        {
            if (!long.TryParse (Uln, out var uln))
                throw new Exception ("Invalid ULN");

            var report = await learnerReportProvider.BuildLearnerReport(uln);

            LearnerName = report.Learner.Name;
            NewCollectionPeriods = report.DataLocks;
            ProviderName = report.Provider.Name;
            ProviderId = report.Provider.Id;
            EmployerName = report.Employer.Name;
            EmployerId = report.Employer.Id;
        }
    }
}