using System;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Payments.Messages.Core.Events;

namespace SFA.DAS.IdentifyDataLocks.Web.Infrastructure
{
    public class DataLockService
    {
        private readonly PaymentsDataContext archiveContext;
        private readonly PaymentsDataContext currentPeriodContext;

        public DataLockService(ArchiveContextFactory archiveContextFactory, CurrentPeriodContextFactory currentPeriodContextFactory)
        {
            this.archiveContext = archiveContextFactory.CreateDbContext();
            this.currentPeriodContext = currentPeriodContextFactory.CreateDbContext();
        }

        public async Task<ApprenticeshipModel?> GetActiveApprenticeship(long uln)
        {
            var statuses = new [] {ApprenticeshipStatus.Active, ApprenticeshipStatus.Paused};
            return await archiveContext.Apprenticeship
                .Include(x => x.ApprenticeshipPriceEpisodes)
                .Include(x => x.ApprenticeshipPauses)
                .Where(x => x.Uln == uln)
                .FirstOrDefaultAsync(a =>
                    statuses.Contains(a.Status));
        }

        public async Task<(IEnumerable<EarningEventModel>, IEnumerable<DataLockEventModel>)> GetLearnerData(long uln, int[] academicYears)
        {
            var earnings = await archiveContext.EarningEvent
                .Include(x => x.PriceEpisodes)
                .Where(x => x.LearnerUln == uln)
                .Where(x => academicYears.Contains(x.AcademicYear))
            .ToListAsync();

            var currentPeriodEarnings = await currentPeriodContext.EarningEvent
                .Include(x => x.PriceEpisodes)
                .Where(x => x.LearnerUln == uln)
                .Where(x => academicYears.Contains(x.AcademicYear))
                .ToListAsync();

            earnings.AddRange(currentPeriodEarnings);

            var locks = await archiveContext.DataLockEvent
                .Include(x => x.NonPayablePeriods)
                .ThenInclude(x => x.DataLockEventNonPayablePeriodFailures)
                .Where(x => x.LearnerUln == uln)
                .Where(x => academicYears.Contains(x.AcademicYear))
                .ToListAsync();

            var currentPeriodLocks = await currentPeriodContext.DataLockEvent
                .Include(x => x.NonPayablePeriods)
                .ThenInclude(x => x.DataLockEventNonPayablePeriodFailures)
                .Where(x => x.LearnerUln == uln)
                .Where(x => academicYears.Contains(x.AcademicYear))
                .ToListAsync();

            locks.AddRange(currentPeriodLocks);

            return (earnings, locks);
        }

        public async Task<IEnumerable<DataLockEventModel>> GetDataLocks(long uln)
        {
            var dataLocks = await archiveContext
                .DataLockEvent
                .Include(x => x.NonPayablePeriods)
                .ThenInclude(x => x.DataLockEventNonPayablePeriodFailures)
                .Where(x => x.LearnerUln == uln)
                .Where(x => x.NonPayablePeriods.Any())
                .OrderByDescending(x => x.CollectionPeriod)
                .ToListAsync();

            var currentPeriodDataLocks = await currentPeriodContext
                .DataLockEvent
                .Include(x => x.NonPayablePeriods)
                .ThenInclude(x => x.DataLockEventNonPayablePeriodFailures)
                .Where(x => x.LearnerUln == uln)
                .Where(x => x.NonPayablePeriods.Any())
                .OrderByDescending(x => x.CollectionPeriod)
                .ToListAsync();

            dataLocks.AddRange(currentPeriodDataLocks);

            return dataLocks;
        }
    }
}