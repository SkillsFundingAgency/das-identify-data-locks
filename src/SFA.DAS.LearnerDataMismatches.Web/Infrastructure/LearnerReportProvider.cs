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

        public async Task<LearnerReport> BuildLearnerReport(long uln, int[] academicYears)
        {
            var activeApprenticeship = await dataLockService.GetActiveApprenticeship(uln);
            if (activeApprenticeship == null) return new LearnerReport();

            var (earnings, dataLocks) = await dataLockService.GetLearnerData(activeApprenticeship, academicYears);
            var providerName = providerService.GetProviderName(activeApprenticeship.Ukprn);
            var (employerName, employerId) = await employerService.GetEmployerName(activeApprenticeship.AccountId);
            var learnerName = await commitmentsService.GetApprenticesName(uln.ToString(), activeApprenticeship.AccountId);

            var periods = new CollectionPeriodReport(activeApprenticeship, earnings, dataLocks);

            return new LearnerReport
            {
                DataLocks = periods.CollectionPeriods,
                Learner = (uln.ToString(), learnerName),
                Employer = (employerId, employerName),
                Provider = (activeApprenticeship.Ukprn.ToString(), providerName),
            };
        }
    }
}