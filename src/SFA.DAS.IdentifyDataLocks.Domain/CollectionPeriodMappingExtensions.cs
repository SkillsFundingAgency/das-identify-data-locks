using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.IdentifyDataLocks.Data.Model;

namespace SFA.DAS.IdentifyDataLocks.Domain
{
    public static class CollectionPeriodMappingExtensions
    {
        public static CollectionPeriod ToCollectionPeriod(this EarningEventModel earning, ApprenticeshipModel apprenticeship, IEnumerable<DataLockFailureModel> dataLockFailures)
        {
            return new CollectionPeriod
            {
                Period = new Period(earning.AcademicYear, earning.CollectionPeriod),
                DataLockErrorCodes = dataLockFailures.GetErrorCodes(earning),
                ApprenticeshipDataMatch = apprenticeship?.ToApprenticeshipDataMatch(),
                IlrEarningDataMatch = earning.ToEarningDataMatch(),
            };
        }

        private static DataMatch ToApprenticeshipDataMatch(this ApprenticeshipModel apprenticeship)
        {
            var apprenticeshipPause = apprenticeship.ApprenticeshipPauses?.OrderByDescending(p => p.PauseDate).Take(1).FirstOrDefault();

            return new DataMatch
            {
                Ukprn = apprenticeship.Ukprn,
                Uln = apprenticeship.Uln,
                Standard = apprenticeship.StandardCode,
                Framework = apprenticeship.FrameworkCode,
                Program = apprenticeship.ProgrammeType,
                Pathway = apprenticeship.PathwayCode,
                StoppedOn = apprenticeship.StopDate,
                CompletionStatus = apprenticeship.Status,

                Cost = apprenticeship.ApprenticeshipPriceEpisodes.Sum(y => y.Cost),
                PriceStart = apprenticeship.ApprenticeshipPriceEpisodes.FirstOrDefault()?.StartDate,
                PausedOn = apprenticeshipPause?.PauseDate,
                ResumedOn = apprenticeshipPause?.ResumeDate,
            };
        }

        private static DataMatch ToEarningDataMatch(this EarningEventModel earning)
        {
            return new DataMatch
            {
                Ukprn = earning.Ukprn,
                Uln = earning.LearnerUln,
                Standard = earning.LearningAimStandardCode,
                Framework = earning.LearningAimFrameworkCode,
                Program = earning.LearningAimProgrammeType,
                Pathway = earning.LearningAimPathwayCode,
                IlrSubmissionDate = earning.IlrSubmissionDateTime,

                Cost = earning.CalculateCost(),
                PriceStart = earning.PriceEpisodes.FirstOrDefault()?.StartDate,
                StoppedOn = earning.PriceEpisodes.FirstOrDefault()?.ActualEndDate,
                Tnp1 = earning.GetTnpValue(x => x.TotalNegotiatedPrice1),
                Tnp2 = earning.GetTnpValue(x => x.TotalNegotiatedPrice2),
                Tnp3 = earning.GetTnpValue(x => x.TotalNegotiatedPrice3),
                Tnp4 = earning.GetTnpValue(x => x.TotalNegotiatedPrice4),
            };
        }
        
        private static List<AmountFromDate> GetTnpValue(this EarningEventModel earning, Func<EarningEventPriceEpisodeModel, decimal> tnpSelector)
        {
            return earning.PriceEpisodes.Select(pe => new AmountFromDate(pe.StartDate, tnpSelector(pe))).ToList();
        }

        private static decimal CalculateCost(this EarningEventModel earning)
        {
            var tnp1And2 = earning.PriceEpisodes.Sum(e => e.TotalNegotiatedPrice1 + e.TotalNegotiatedPrice2);

            var tnp3And4 = earning.PriceEpisodes.Sum(e => e.TotalNegotiatedPrice3 + e.TotalNegotiatedPrice4);

            return tnp3And4 > 0 ? tnp3And4 : tnp1And2;
        }

        private static List<DataLockErrorCode> GetErrorCodes(this IEnumerable<DataLockFailureModel> locks, EarningEventModel earning)
        {
            return locks
                .Where(l => l.Ukprn == earning.Ukprn && l.AcademicYear == earning.AcademicYear && l.CollectionPeriod == earning.CollectionPeriod)
                .SelectMany(l => l.DataLockFailures)
                .Distinct()
                .OrderBy(x => x)
                .ToList();
        }
    }
}