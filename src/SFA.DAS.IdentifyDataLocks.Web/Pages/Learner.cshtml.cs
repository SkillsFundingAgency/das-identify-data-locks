using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.IdentifyDataLocks.Domain;
using SFA.DAS.IdentifyDataLocks.Domain.Services;
using SFA.DAS.IdentifyDataLocks.Web.Helpers;

namespace SFA.DAS.IdentifyDataLocks.Web.Pages
{
    public class LearnerModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Uln { get; set; }
        public IEnumerable<CollectionPeriod> CurrentYearDataLocks { get; set; } = Enumerable.Empty<CollectionPeriod>();
        public IEnumerable<CollectionPeriod> PreviousYearDataLocks { get; set; } = Enumerable.Empty<CollectionPeriod>();
        public string LearnerName { get; private set; }
        public string EmployerName { get; private set; }
        public string EmployerId { get; private set; }
        public string ProviderName { get; private set; }
        public string ProviderId { get; private set; }
        public bool HasDataLocks { get; private set; }
        public bool HasMultipleProviders { get; set; }

        public IEnumerable<string> DataLockNames
        {
            get
            {
                return CurrentYearDataLocks
                    .SelectMany(x => x.DataLockErrorCodes)
                    .Distinct()
                    .Select(x => x.ToString())
                    .OrderBy(x => x);
            }
        }

        public IEnumerable<DataLockHelpCentreLink> DataLockLinks
        {
            get
            {
                return CurrentYearDataLocks
                    .Concat(PreviousYearDataLocks)
                    .SelectMany(x => x.DataLockErrorCodes)
                    .Distinct()
                    .Select(DataLockHelpCentreLink.Create)
                    .OrderBy(x => x.Name);
            }
        }

        public bool HasDataLocksInCurrentYear => CurrentYearDataLocks.Any();

        public bool HasDataLocksInPreviousYear => PreviousYearDataLocks.Any();

        public (AcademicYear Current, AcademicYear Previous) AcademicYears => (new AcademicYear(_timeProvider.Today), new AcademicYear(_timeProvider.Today) - 1);

        private readonly LearnerReportProvider _learnerReportProvider;
        private readonly ITimeProvider _timeProvider;

        public LearnerModel(LearnerReportProvider learnerReportProvider, ITimeProvider timeProvider)
        {
            _learnerReportProvider = learnerReportProvider;
            _timeProvider = timeProvider;
        }

        public async Task OnGetAsync()
        {
            if (!long.TryParse(Uln, out var uln))
                throw new Exception("Invalid ULN");

            var report = await _learnerReportProvider.BuildLearnerReport(uln, AcademicYears);

            HasDataLocks = report.HasDataLocks;
            HasMultipleProviders = report.HasMultipleProviders;
            LearnerName = report.Learner.Name;
            ProviderName = report.Provider.Name;
            ProviderId = report.Provider.Id;
            EmployerName = report.Employer.Name;
            EmployerId = report.Employer.Id;
            CurrentYearDataLocks = report.CurrentYearDataLocks;
            PreviousYearDataLocks = report.PreviousYearDataLocks;
        }
    }
}