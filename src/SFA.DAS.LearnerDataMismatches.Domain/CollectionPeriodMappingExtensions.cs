using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.LearnerDataMismatches.Domain
{
    public static class CollectionPeriodMappingExtensions
    {
        public static CollectionPeriod ToCollectionPeriod(
            this EarningEventModel earning,
            ApprenticeshipModel? apprenticeship,
            IEnumerable<DataLockEventModel> datalocks)
            =>
            new CollectionPeriod
            {
                Period = new Period(earning.AcademicYear, earning.CollectionPeriod),
                DataLocks = ToDataLocks(datalocks, earning),
                Apprenticeship = apprenticeship?.ToDataMatch(),
                Ilr = earning.ToDataMatch(),
            };

        private static DataMatch ToDataMatch(this ApprenticeshipModel apprenticeship) =>
            new DataMatch
            {
                Ukprn = apprenticeship.Ukprn,
                Uln = apprenticeship.Uln,
                Standard = (short?)apprenticeship.StandardCode,
                Framework = (short?)apprenticeship.FrameworkCode,
                Program = (short?)apprenticeship.ProgrammeType,
                Pathway = (short?)apprenticeship.PathwayCode,
                Cost = apprenticeship.ApprenticeshipPriceEpisodes.Sum(y => y.Cost),
                PriceStart = apprenticeship.ApprenticeshipPriceEpisodes.FirstOrDefault()?.StartDate,
                StoppedOn = apprenticeship.StopDate,
                CompletionStatus = (ApprenticeshipStatus)apprenticeship.Status,
            };

        private static DataMatch ToDataMatch(this EarningEventModel earning) =>
            new DataMatch
            {
                Uln = earning.LearnerUln,
                Ukprn = earning.Ukprn,
                Standard = (short)earning.LearningAimStandardCode,
                Framework = (short)earning.LearningAimFrameworkCode,
                Program = (short)earning.LearningAimProgrammeType,
                Pathway = (short)earning.LearningAimPathwayCode,
                Cost = earning.PriceEpisodes.Sum(e =>
                    e.TotalNegotiatedPrice1 +
                    e.TotalNegotiatedPrice2 +
                    e.TotalNegotiatedPrice3 +
                    e.TotalNegotiatedPrice4),
                PriceStart = earning.PriceEpisodes.FirstOrDefault()?.StartDate,
                StoppedOn = earning.PriceEpisodes.FirstOrDefault()?.ActualEndDate,
                //CompletionStatus = (Domain.ApprenticeshipStatus)x.Status,
            };

        private static List<DataLock> ToDataLocks(this IEnumerable<DataLockEventModel> locks, EarningEventModel earning) =>
            locks
                .Where(l => l.Ukprn == earning.Ukprn && l.AcademicYear == earning.AcademicYear && l.CollectionPeriod == earning.CollectionPeriod)
                .SelectMany(l => l.NonPayablePeriods)
                .SelectMany(l => l.DataLockEventNonPayablePeriodFailures)
                .Select(l => (DataLock)l.DataLockFailure)
                .Distinct()
                .OrderBy(x => x)
                .ToList();
    }
}