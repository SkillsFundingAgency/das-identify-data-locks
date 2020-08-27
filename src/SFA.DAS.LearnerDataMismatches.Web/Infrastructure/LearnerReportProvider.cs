using System.Threading.Tasks;
using SFA.DAS.LearnerDataMismatches.Domain;

namespace SFA.DAS.LearnerDataMismatches.Web.Infrastructure 
{
    public interface ILearnerReportProvider
    {
        Task<LearnerReport> GetLearnerReport(long uln);
    }

    public class LearnerReportProvider : ILearnerReportProvider
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

        public async Task<LearnerReport> GetLearnerReport(long uln)
        {
            var learnerReport = new LearnerReport();

            var activeApprenticeship = await dataLockService.GetActiveApprenticeship(uln);
            if (activeApprenticeship == null) return learnerReport;

            learnerReport.ActiveApprenticeship = activeApprenticeship;
            
            learnerReport.DataLocks = await dataLockService.GetAllActiveDataLocks(activeApprenticeship);

            learnerReport.ProviderId = activeApprenticeship.Ukprn.ToString ();
            learnerReport.ProviderName = providerService.GetProviderName(activeApprenticeship.Ukprn);

            (learnerReport.EmployerName, learnerReport.EmployerId) = await employerService.GetEmployerName(activeApprenticeship.AccountId);

            learnerReport.LearnerName = await commitmentsService.GetApprenticesName(uln.ToString(), activeApprenticeship.AccountId);

            return learnerReport;
        }
    }
}