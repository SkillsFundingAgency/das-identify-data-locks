using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.LearnerDataMismatches.Domain
{
    public class CollectionPeriodReport
    {
        public CollectionPeriodReport(ApprenticeshipModel activeApprenticeship, IEnumerable<EarningEventModel> earnings, IEnumerable<DataLockEventModel> locks)
        {
            CollectionPeriods = earnings
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
                    CompletionStatus = (ApprenticeshipStatus)activeApprenticeship.Status,
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

        public IEnumerable<CollectionPeriod> CollectionPeriods { get; }
    }
}