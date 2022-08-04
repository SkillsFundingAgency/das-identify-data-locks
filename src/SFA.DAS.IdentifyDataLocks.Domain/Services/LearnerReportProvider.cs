using System.Threading.Tasks;

namespace SFA.DAS.IdentifyDataLocks.Domain.Services
{
    public class LearnerReportProvider
    {
        private readonly DataLockService _dataLockService;
        private readonly EmployerService _employerService;
        private readonly IProviderService _providerService;
        private readonly CommitmentsService _commitmentsService;

        public LearnerReportProvider(
            DataLockService dataLockService,
            EmployerService employerService,
            IProviderService providerService,
            CommitmentsService commitmentsService)
        {
            _commitmentsService = commitmentsService;
            _providerService = providerService;
            _employerService = employerService;
            _dataLockService = dataLockService;
        }

        public async Task<LearnerReport> BuildLearnerReport(long uln, (AcademicYear current, AcademicYear previous) academicYears)
        {
            var (activeApprenticeship, earnings, dataLocks) =
                await _dataLockService.GetLearnerData(uln, new int[] { academicYears.current, academicYears.previous });

            var learnerReport = new LearnerReport(activeApprenticeship, earnings, dataLocks, academicYears);

            if (activeApprenticeship != null)
            {
                var providerName = await _providerService.GetProvider(activeApprenticeship.Ukprn);
                var (employerName, employerId) = await _employerService.GetEmployerName(activeApprenticeship.AccountId);
                var learnerName = await _commitmentsService.GetApprenticesName(uln.ToString(), activeApprenticeship.AccountId);

                learnerReport.Learner = (uln.ToString(), learnerName);
                learnerReport.Employer = (employerId, employerName);
                learnerReport.Provider = (activeApprenticeship.Ukprn.ToString(), providerName);
            }

            return learnerReport;
        }
    }
}