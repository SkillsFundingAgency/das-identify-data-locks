using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.IdentifyDataLocks.Web.Infrastructure
{
    public class DataLockService
    {
        private readonly IPaymentsDataContext context;

        public DataLockService(IPaymentsDataContext context) =>
            this.context = context;

        public async Task<ApprenticeshipModel?> GetActiveApprenticeship(long uln)
        {
            var statuses = new [] {ApprenticeshipStatus.Active, ApprenticeshipStatus.Paused};
            return await context.Apprenticeship
                .Include(x => x.ApprenticeshipPriceEpisodes)
                .Include(x => x.ApprenticeshipPauses)
                .Where(x => x.Uln == uln)
                .FirstOrDefaultAsync(a =>
                    statuses.Contains(a.Status));
        }

        public async Task<(IEnumerable<EarningEventModel>, IEnumerable<DataLockEventModel>)> GetLearnerData(long uln, int[] academicYears)
        {
            var earnings = await context.EarningEvent
                .Include(x => x.PriceEpisodes)
                .Where(x => x.LearnerUln == uln)
                .Where(x => academicYears.Contains(x.AcademicYear))
                .ToListAsync();

            var locks = await context.DataLockEvent
                .Include(x => x.NonPayablePeriods)
                .ThenInclude(x => x.DataLockEventNonPayablePeriodFailures)
                .Where(x => x.LearnerUln == uln)
                .Where(x => academicYears.Contains(x.AcademicYear))
                .ToListAsync();

            return (earnings, locks);
        }
    }
}