using SFA.DAS.LearnerDataMismatches.Domain;
using System.Threading.Tasks;

namespace SFA.DAS.LearnerDataMismatches.Web.Infrastructure
{
    public class LearnerReportProvider
    {
        private readonly DataLockService dataLockService;
        private readonly EmployerService employerService;
        private readonly ProviderService providerService;
        private readonly CommitmentsService commitmentsService;

        public LearnerReportProvider(
            DataLockService dataLockService,
            EmployerService employerService,
            ProviderService providerService,
            CommitmentsService commitmentsService)
        {
            this.commitmentsService = commitmentsService;
            this.providerService = providerService;
            this.employerService = employerService;
            this.dataLockService = dataLockService;
        }

        public async Task<LearnerReport> BuildLearnerReport(long uln, (AcademicYear current, AcademicYear previous) academicYears)
        {
            var activeApprenticeship = await dataLockService.GetActiveApprenticeship(uln);

            var (earnings, dataLocks) = await dataLockService.GetLearnerData(activeApprenticeship.Uln, new int[] {academicYears.current.ShortRepresentation, academicYears.previous.ShortRepresentation});

            var report = new CollectionPeriodReport(activeApprenticeship, earnings, dataLocks);

            var learnerReport = new LearnerReport
            {
                DataLocks = report.CollectionPeriodsByYear,
                HasDataLocks = report.HasDataLocks
            };

            if (activeApprenticeship != null)
            {
                var providerName = providerService.GetProviderName(activeApprenticeship.Ukprn);
                var (employerName, employerId) = await employerService.GetEmployerName(activeApprenticeship.AccountId);
                var learnerName = await commitmentsService.GetApprenticesName(uln.ToString(), activeApprenticeship.AccountId);

                learnerReport.Learner = (uln.ToString(), learnerName);
                learnerReport.Employer = (employerId, employerName);
                learnerReport.Provider = (activeApprenticeship.Ukprn.ToString(), providerName);
            }

            return learnerReport;
        }
    }
}