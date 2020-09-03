using SFA.DAS.LearnerDataMismatches.Domain;
using System.Collections.Generic;
using System.Linq;
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

            var (earnings, dataLocks) = await dataLockService.GetLearnerData(activeApprenticeship.Uln, academicYears);
            var providerName = providerService.GetProviderName(activeApprenticeship.Ukprn);
            var (employerName, employerId) = await employerService.GetEmployerName(activeApprenticeship.AccountId);
            var learnerName = await commitmentsService.GetApprenticesName(uln.ToString(), activeApprenticeship.AccountId);

            var report = new CollectionPeriodReport(activeApprenticeship, earnings, dataLocks);

            var dataLocksByAcademicYear = report.CollectionPeriods.GroupBy(c => c.Period.Year).ToDictionary(g => g.Key, g => g.ToList());

            return new LearnerReport
            {
                DataLocks = dataLocksByAcademicYear,
                Learner = (uln.ToString(), learnerName),
                Employer = (employerId, employerName),
                Provider = (activeApprenticeship.Ukprn.ToString(), providerName),
                HasDataLocks = report.HasDataLocks
            };
        }
    }
}