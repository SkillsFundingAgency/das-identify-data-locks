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

        public IEnumerable<Domain.CollectionPeriod> CurrentYearDataLocks { get; private set; } = Enumerable.Empty<Domain.CollectionPeriod> ();
        public IEnumerable<Domain.CollectionPeriod> PreviousYearDataLocks { get; private set; } = Enumerable.Empty<Domain.CollectionPeriod> ();

        public string LearnerName { get; private set; }
        public string EmployerName { get; private set; }
        public string EmployerId { get; private set; }
        public string ProviderName { get; private set; }
        public string ProviderId { get; private set; }
        public bool HasDataLocks { get; private set; }
        public IEnumerable<string> DataLockNames =>
            CurrentYearDataLocks
            .SelectMany (x => x.DataLocks)
            .Select (x => x.ToString ())
            .OrderBy (x => x);

        public IEnumerable<DataLockHelpCentreLink> DataLockLinks =>
            CurrentYearDataLocks
                .Concat(PreviousYearDataLocks)
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

        public async Task OnGetAsync() 
        {
            if (!long.TryParse (Uln, out var uln))
                throw new Exception ("Invalid ULN");

            var report = await learnerReportProvider.BuildLearnerReport(uln, new [] {AcademicYear.Current, AcademicYear.Previous});

            HasDataLocks = report.HasDataLocks;
            LearnerName = report.Learner.Name;
            ProviderName = report.Provider.Name;
            ProviderId = report.Provider.Id;
            EmployerName = report.Employer.Name;
            EmployerId = report.Employer.Id;
            if (report.HasDataLocks && report.DataLocks.ContainsKey(AcademicYear.Current)) CurrentYearDataLocks = report.DataLocks[AcademicYear.Current];
            if (report.HasDataLocks && report.DataLocks.ContainsKey(AcademicYear.Previous)) PreviousYearDataLocks = report.DataLocks[AcademicYear.Previous];
        }
    }
}