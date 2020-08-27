using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.LearnerDataMismatches.Domain
{
    public class LearnerReport
    {
        public LearnerReport(
            List<Payments.Model.Core.Entities.ApprenticeshipModel> apprenticeships,
            List<Payments.Model.Core.Audit.EarningEventModel> earnings,
            List<Payments.Model.Core.Audit.DataLockEventModel> locks)
        {
            var apps = apprenticeships
                .Where(x => x.Status == Payments.Model.Core.Entities.ApprenticeshipStatus.Active)
                .Select(x => new DataMatch
                {
                    Ukprn = x.Ukprn,
                    Uln = x.Uln,
                    Standard = (short)x.StandardCode.Value,
                    Framework = (short)x.FrameworkCode.Value,
                    Program = (short)x.ProgrammeType.Value,
                    Pathway = (short)x.PathwayCode.Value,
                    Cost = x.ApprenticeshipPriceEpisodes.Sum(y => y.Cost),
                    PriceStart = x.ApprenticeshipPriceEpisodes.FirstOrDefault()?.StartDate,
                    CompletionStatus = (ApprenticeshipStatus)x.Status,
                })
                .ToList();

            CollectionPeriods = earnings
                .Where(x => apps.Select(y => y.Ukprn).Contains(x.Ukprn))
                .Select(x => new CollectionPeriod
                {
                    DataLocks = locks
                    .Where(l => l.Ukprn == x.Ukprn && l.CollectionPeriod == x.CollectionPeriod)
                    .SelectMany(l => l.NonPayablePeriods)
                    .SelectMany(l => l.DataLockEventNonPayablePeriodFailures)
                    .Select(l => (DataLock)l.DataLockFailure)
                    .ToList(),

                    Apprenticeship = apps.Find(a => a.Uln == x.LearnerUln && a.Ukprn == x.Ukprn),

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
                        //CompletionStatus = (Domain.ApprenticeshipStatus)x.Status,
                    },
                    Period = new Period(x.AcademicYear, x.CollectionPeriod),
                })
                .GroupBy(x => x.Period)
                .Select(x => x.First())
                .OrderByDescending(x => x)
                .ToList();
        }

        public IEnumerable<CollectionPeriod> CollectionPeriods { get; set; }
    }
}
