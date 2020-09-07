using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.LearnerDataMismatches.Domain
{
    public class CollectionPeriodReport
    {
        public CollectionPeriodReport(
            ApprenticeshipModel activeApprenticeship,
            IEnumerable<EarningEventModel> earnings,
            IEnumerable<DataLockEventModel> locks)
        {
            HasDataLocks = locks.Any();

            //activeApprenticeship ??= new ApprenticeshipModel
            //{
            //    Ukprn = earnings.FirstOrDefault()?.Ukprn ?? 0,
            //};

            var dataMatch = activeApprenticeship != null
                ? new DataMatch
                {
                    Ukprn = activeApprenticeship.Ukprn,
                    Uln = activeApprenticeship.Uln,
                    Standard = (short?)activeApprenticeship.StandardCode,
                    Framework = (short?)activeApprenticeship.FrameworkCode,
                    Program = (short?)activeApprenticeship.ProgrammeType,
                    Pathway = (short?)activeApprenticeship.PathwayCode,
                    Cost = activeApprenticeship.ApprenticeshipPriceEpisodes.Sum(y => y.Cost),
                    PriceStart = activeApprenticeship.ApprenticeshipPriceEpisodes.FirstOrDefault()?.StartDate,
                    StoppedOn = activeApprenticeship.StopDate,
                    CompletionStatus = (ApprenticeshipStatus)activeApprenticeship.Status,
                }
                : null;

            bool IncludeEverything(EarningEventModel _) => true;
            bool FilterActive(EarningEventModel x) => x.Ukprn == activeApprenticeship?.Ukprn;

            var filter = earnings.Select(x => x.Ukprn).Distinct().Count() == 1
                ? (Func<EarningEventModel, bool>)IncludeEverything
                : (Func<EarningEventModel, bool>)FilterActive;

            var providerEarnings = earnings.Where(filter);

            CollectionPeriods = providerEarnings
                .Select(x => new CollectionPeriod
                {
                    DataLocks = locks
                        .Where(l => l.Ukprn == x.Ukprn && l.AcademicYear == x.AcademicYear && l.CollectionPeriod == x.CollectionPeriod)
                        .SelectMany(l => l.NonPayablePeriods)
                        .SelectMany(l => l.DataLockEventNonPayablePeriodFailures)
                        .Select(l => (DataLock)l.DataLockFailure)
                        .Distinct()
                        .OrderBy(x => x)
                        .ToList(),

                    Apprenticeship = dataMatch,

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
                //.DefaultIfEmpty(new CollectionPeriod
                //{
                //    Apprenticeship = dataMatch,
                //    DataLocks = locks
                //        .Where(l => l.Ukprn == activeApprenticeship.Ukprn)
                //        .SelectMany(l => l.NonPayablePeriods)
                //        .SelectMany(l => l.DataLockEventNonPayablePeriodFailures)
                //        .Select(l => (DataLock)l.DataLockFailure)
                //        .Distinct()
                //        .OrderBy(x => x)
                //        .ToList(),
                //})
                .ToList();
        }

        public IEnumerable<CollectionPeriod> CollectionPeriods { get; }

        public Dictionary<AcademicYear, List<CollectionPeriod>> CollectionPeriodsByYear =>
            CollectionPeriods
                .GroupBy(c => c.Period.Year)
                .ToDictionary(g => (AcademicYear)g.Key, g => g.ToList());

        public bool HasDataLocks { get; }
    }
}