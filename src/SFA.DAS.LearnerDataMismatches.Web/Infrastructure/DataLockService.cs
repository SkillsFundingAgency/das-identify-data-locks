using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.LearnerDataMismatches.Web.Infrastructure
{
    public class DataLockService
    {
        private readonly IPaymentsDataContext context;

        public DataLockService(IPaymentsDataContext context) =>
            this.context = context;

        public async Task<ApprenticeshipModel> GetActiveApprenticeship(long uln)
        {
            return await context.Apprenticeship
                .Include(x => x.ApprenticeshipPriceEpisodes)
                .Where(x => x.Uln == uln)
                .FirstOrDefaultAsync(a =>
                    a.Status == ApprenticeshipStatus.Active);
        }

        public async Task<(IEnumerable<EarningEventModel>, IEnumerable<DataLockEventModel>)> GetLearnerData(ApprenticeshipModel apprenticeship, int[] academicYears)
        {
            var earnings = await context.EarningEvent
                .Include(x => x.PriceEpisodes)
                .Where(x => x.LearnerUln == apprenticeship.Uln)
                .Where(x => academicYears.Contains((x.AcademicYear)))
                .ToListAsync();

            var locks = await context.DataLockEvent
                .Include(x => x.NonPayablePeriods)
                .ThenInclude(x => x.DataLockEventNonPayablePeriodFailures)
                .Where(x => x.LearnerUln == apprenticeship.Uln)
                .Where(x => academicYears.Contains(x.AcademicYear))
                .ToListAsync();

            return (earnings, locks);
        }
    }
}