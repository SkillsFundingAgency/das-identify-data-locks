using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.LearnerDataMismatches.Domain
{
    public class LearnerReport
    {
        private List<Payments.Model.Core.Entities.ApprenticeshipModel> apps2;
        private List<Payments.Model.Core.Audit.EarningEventModel> earnings;

        public LearnerReport(List<Payments.Model.Core.Entities.ApprenticeshipModel> apprenticeships, List<Payments.Model.Core.Audit.EarningEventModel> earnings)
        {
            this.apps2 = apprenticeships;
            this.earnings = earnings;

            var apps = apps2
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

            CollectionPeriods = earnings.Select(x => new CollectionPeriod
            {
                Apprenticeship = apps.Find(a => a.Uln == x.LearnerUln && a.Ukprn == x.Ukprn),
                Ilr = new DataMatch
                {
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
            }).ToList();
        }

        public IEnumerable<CollectionPeriod> CollectionPeriods { get; set; }
    }
}
