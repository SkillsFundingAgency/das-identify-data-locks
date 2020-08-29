using SFA.DAS.LearnerDataMismatches.Domain;
using System.Threading.Tasks;

namespace SFA.DAS.LearnerDataMismatches.Web.Infrastructure
{
    public class LearnerReportProvider
    {
        private readonly IDataLockService dataLockService;
        private readonly IEmployerService employerService;
        private readonly IProviderService providerService;
        private readonly ICommitmentsService commitmentsService;

        public LearnerReportProvider(IDataLockService dataLockService, IEmployerService employerService, IProviderService providerService, ICommitmentsService commitmentsService)
        {
            this.commitmentsService = commitmentsService;
            this.providerService = providerService;
            this.employerService = employerService;
            this.dataLockService = dataLockService;
        }

        public async Task<LearnerReport> BuildLearnerReport(long uln)
        {
            var activeApprenticeship = await dataLockService.GetActiveApprenticeship(uln);
            if (activeApprenticeship == null) return new LearnerReport();

            var (earnings, dlocks) = await dataLockService.LoadLearnerData(activeApprenticeship);
            var providerName = providerService.GetProviderName(activeApprenticeship.Ukprn);
            var (employerName, employerId) = await employerService.GetEmployerName(activeApprenticeship.AccountId);
            var learnerName = await commitmentsService.GetApprenticesName(uln.ToString(), activeApprenticeship.AccountId);

            var periods = new CollectionPeriodReport(activeApprenticeship, earnings, dlocks);

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