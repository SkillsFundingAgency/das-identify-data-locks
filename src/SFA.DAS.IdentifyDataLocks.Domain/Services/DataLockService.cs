using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.IdentifyDataLocks.Data.Model;
using SFA.DAS.IdentifyDataLocks.Data.Repositories;

namespace SFA.DAS.IdentifyDataLocks.Domain.Services
{
    public class DataLockService
    {
        private readonly PaymentsDataContext _paymentsDataContext;
        private readonly PaymentsAuditDataContext _paymentsAuditDataContext;

        public DataLockService(PaymentsDataContext paymentsDataContext, PaymentsAuditDataContext paymentsAuditDataContext)
        {
            _paymentsDataContext = paymentsDataContext;
            _paymentsAuditDataContext = paymentsAuditDataContext;
        }

        public async Task<(ApprenticeshipModel, IList<EarningEventModel>, IList<DataLockFailureModel>)> GetLearnerData(long uln, int[] academicYears)
        {
            var currentLearnerData = await GetLearnerDataInternal(_paymentsDataContext, uln, academicYears);
            var historicalLearnerData = await GetLearnerDataInternal(_paymentsAuditDataContext, uln, academicYears);

            return (currentLearnerData.Item1, currentLearnerData.Item2.Concat(historicalLearnerData.Item2).Distinct().ToList(), currentLearnerData.Item3.Concat(historicalLearnerData.Item3).Distinct().ToList());
        }

        private async Task<(ApprenticeshipModel, IList<EarningEventModel>, IList<DataLockFailureModel>)> GetLearnerDataInternal(IPaymentsDataContext context, long uln, int[] academicYears)
        {
            var statuses = new[] { ApprenticeshipStatus.Active, ApprenticeshipStatus.Paused };

            var activeApprenticeship = await context.Apprenticeship
                .Include(x => x.ApprenticeshipPriceEpisodes)
                .Include(x => x.ApprenticeshipPauses)
                .Where(x => x.Uln == uln)
                .Where(x => statuses.Contains(x.Status))
                .FirstOrDefaultAsync();

            var earnings = await context.EarningEvent
                .Include(x => x.PriceEpisodes)
                .Where(x => x.LearnerUln == uln)
                .Where(e => e.LearningAimReference == "ZPROG001")
                .Where(x => academicYears.Contains(x.AcademicYear))
                .ToListAsync();

            var locks = await context.DataLockEvent
                .Include(x => x.NonPayablePeriods)
                .ThenInclude(x => x.DataLockEventNonPayablePeriodFailures)
                .Where(x => x.LearnerUln == uln)
                .Where(x => academicYears.Contains(x.AcademicYear))
                .GroupBy(g => new { g.AcademicYear, g.CollectionPeriod, g.Ukprn })
                .Select(gl => new DataLockFailureModel
                {
                    Ukprn = gl.Key.Ukprn,
                    CollectionPeriod = gl.Key.CollectionPeriod,
                    AcademicYear = gl.Key.AcademicYear,
                    DataLockFailures = gl.SelectMany(d => d.NonPayablePeriods)
                        .SelectMany(f => f.DataLockEventNonPayablePeriodFailures)
                        .Select(e => e.DataLockFailure)
                        .Distinct()
                        .OrderBy(x => x)
                        .ToList()
                })
                .ToListAsync();


            return (activeApprenticeship, earnings, locks);
        }
    }
}