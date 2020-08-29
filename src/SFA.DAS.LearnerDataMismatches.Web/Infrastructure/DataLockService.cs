using Microsoft.EntityFrameworkCore;
using SFA.DAS.LearnerDataMismatches.Domain;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.LearnerDataMismatches.Web.Infrastructure
{
    public interface IDataLockService
    {
        Task<ApprenticeshipModel> GetActiveApprenticeship(long uln);

        Task<IEnumerable<CollectionPeriod>> GetAllActiveDataLocks(ApprenticeshipModel activeApprenticeship);

        Task<(IEnumerable<EarningEventModel> earnings, IEnumerable<DataLockEventModel> dlocks)> LoadLearnerData(ApprenticeshipModel apprenticeship);
    }

    public class DataLockService : IDataLockService
    {
        private readonly IPaymentsDataContext _context;

        public DataLockService(IPaymentsDataContext context)
        {
            _context = context;
        }

        public async Task<ApprenticeshipModel> GetActiveApprenticeship(long uln)
        {
            return await _context.Apprenticeship
                .Include(x => x.ApprenticeshipPriceEpisodes)
                .Where(x => x.Uln == uln)
                .FirstOrDefaultAsync(a =>
                    a.Status == Payments.Model.Core.Entities.ApprenticeshipStatus.Active);
        }

        public
        async Task<(IEnumerable<EarningEventModel> earnings, IEnumerable<DataLockEventModel> dlocks)>
        LoadLearnerData(ApprenticeshipModel apprenticeship)
        {
            var earnings = await _context.EarningEvent
                .Include(x => x.PriceEpisodes)
                .Where(x => x.LearnerUln == apprenticeship.Uln)
                .ToListAsync();

            var locks = await _context.DataLockEvent
                .Include(x => x.NonPayablePeriods)
                .ThenInclude(x => x.DataLockEventNonPayablePeriodFailures)
                .Where(x => x.LearnerUln == apprenticeship.Uln)
                .ToListAsync();

            return (earnings, locks);
        }

        public async Task<IEnumerable<CollectionPeriod>> GetAllActiveDataLocks(ApprenticeshipModel activeApprenticeship)
        {
            var earnings = await _context.EarningEvent
                .Include(x => x.PriceEpisodes)
                .Where(x => x.LearnerUln == activeApprenticeship.Uln)
                .ToListAsync();

            var locks = await _context.DataLockEvent
                .Include(x => x.NonPayablePeriods)
                .ThenInclude(x => x.DataLockEventNonPayablePeriodFailures)
                .Where(x => x.LearnerUln == activeApprenticeship.Uln)
                .ToListAsync();

            return GetAllActiveDataLock(activeApprenticeship, earnings, locks);
        }

        private IEnumerable<CollectionPeriod> GetAllActiveDataLock(
            Payments.Model.Core.Entities.ApprenticeshipModel activeApprenticeship,
            List<Payments.Model.Core.Audit.EarningEventModel> earnings,
            List<Payments.Model.Core.Audit.DataLockEventModel> locks)
        {
            return earnings
                .Where(x => x.Ukprn == activeApprenticeship.Ukprn)
                .Select(x => new CollectionPeriod
                {
                    DataLocks = locks
                    .Where(l => l.Ukprn == x.Ukprn && l.CollectionPeriod == x.CollectionPeriod)
                    .SelectMany(l => l.NonPayablePeriods)
                    .SelectMany(l => l.DataLockEventNonPayablePeriodFailures)
                    .Select(l => (DataLock)l.DataLockFailure)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList(),

                    Apprenticeship = new DataMatch
                    {
                        Ukprn = activeApprenticeship.Ukprn,
                        Uln = activeApprenticeship.Uln,
                        Standard = (short)activeApprenticeship.StandardCode.Value,
                        Framework = (short)activeApprenticeship.FrameworkCode.Value,
                        Program = (short)activeApprenticeship.ProgrammeType.Value,
                        Pathway = (short)activeApprenticeship.PathwayCode.Value,
                        Cost = activeApprenticeship.ApprenticeshipPriceEpisodes.Sum(y => y.Cost),
                        PriceStart = activeApprenticeship.ApprenticeshipPriceEpisodes.FirstOrDefault()?.StartDate,
                        StoppedOn = activeApprenticeship.StopDate,
                        CompletionStatus = (Domain.ApprenticeshipStatus)activeApprenticeship.Status,
                    },

                    Ilr = new DataMatch
                    {
                        Uln = x.LearnerUln,
                        Ukprn = x.Ukprn,
                        Standard = (short)x.LearningAimStandardCode,
                        Framework = (short)x.LearningAimFrameworkCode,
                        Program = (short)x.LearningAimProgrammeType,
                        Pathway = (short)x.LearningAimPathwayCode,
                        Cost = x.PriceEpisodes.Sum(e =>
                            e.TotalNegotiatedPrice1 +
                            e.TotalNegotiatedPrice2 +
                            e.TotalNegotiatedPrice3 +
                            e.TotalNegotiatedPrice4),
                        PriceStart = x.PriceEpisodes.FirstOrDefault()?.StartDate,
                        StoppedOn = x.PriceEpisodes.FirstOrDefault()?.ActualEndDate,
                        //CompletionStatus = (Domain.ApprenticeshipStatus)x.Status,
                    },
                    Period = new Period(x.AcademicYear, x.CollectionPeriod),
                })
                .GroupBy(x => x.Period)
                .Select(x => x.First())
                .OrderByDescending(x => x)
                .ToList();
        }
    }
}