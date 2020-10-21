using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.IdentifyDataLocks.Domain
{
    public static class CollectionPeriodMappingExtensions
    {
        public static CollectionPeriod ToCollectionPeriod(
            this EarningEventModel earning,
            ApprenticeshipModel apprenticeship,
            IEnumerable<DataLockEventModel> datalocks)
            =>
            new CollectionPeriod
            {
                Period = new Period(earning.AcademicYear, earning.CollectionPeriod),
                DataLocks = datalocks.ToDataLocks(earning),
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
                PausedOn = GetPausedOnDate(apprenticeship),
                ResumedOn = GetResumedOnDate(apprenticeship),
            };

        private static DateTime? GetPausedOnDate(ApprenticeshipModel apprenticeship)
        {
            return apprenticeship.ApprenticeshipPauses?.OrderByDescending(p => p.PauseDate).Take(1).FirstOrDefault()?.PauseDate;
        }

        private static DateTime? GetResumedOnDate(ApprenticeshipModel apprenticeship)
        {
            return apprenticeship.ApprenticeshipPauses?.OrderByDescending(p => p.PauseDate).Take(1).FirstOrDefault()?.ResumeDate;
        }

        private static DataMatch ToDataMatch(this EarningEventModel earning) =>
            new DataMatch
            {
                Uln = earning.LearnerUln,
                Ukprn = earning.Ukprn,
                Standard = (short)earning.LearningAimStandardCode,
                Framework = (short)earning.LearningAimFrameworkCode,
                Program = (short)earning.LearningAimProgrammeType,
                Pathway = (short)earning.LearningAimPathwayCode,
                Cost = earning.CalculateCost(),
                PriceStart = earning.PriceEpisodes.FirstOrDefault()?.StartDate,
                StoppedOn = earning.PriceEpisodes.FirstOrDefault()?.ActualEndDate,
                IlrSubmissionDate = earning.IlrSubmissionDateTime,
                Tnp1 = earning.PriceEpisodes.Select(x => (x.StartDate, x.TotalNegotiatedPrice1)).ToList(),
                Tnp2 = earning.PriceEpisodes.FirstOrDefault()?.TotalNegotiatedPrice2,
                Tnp3 = earning.PriceEpisodes.FirstOrDefault()?.TotalNegotiatedPrice3,
                Tnp4 = earning.PriceEpisodes.FirstOrDefault()?.TotalNegotiatedPrice4,
            };

        private static decimal CalculateCost(this EarningEventModel earning)
        {
            var tnp1and2 = earning.PriceEpisodes.Sum(e =>
                e.TotalNegotiatedPrice1 + e.TotalNegotiatedPrice2);

            var tnp3and4 = earning.PriceEpisodes.Sum(e =>
                e.TotalNegotiatedPrice3 + e.TotalNegotiatedPrice4);

            return tnp3and4 > 0 ? tnp3and4 : tnp1and2;
        }

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