using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.IdentifyDataLocks.Domain
{
    public class CollectionPeriodReport
    {
        public bool HasDataLocks { get; }

        public IEnumerable<CollectionPeriod> CollectionPeriods { get; }

        public Dictionary<AcademicYear, List<CollectionPeriod>> CollectionPeriodsByYear =>
            CollectionPeriods
                .GroupBy(c => (AcademicYear)c.Period.Year)
                .ToDictionary(g => g.Key, g => g.ToList());

        public CollectionPeriodReport(
            ApprenticeshipModel activeApprenticeship,
            IEnumerable<EarningEventModel> earnings,
            IEnumerable<DataLockEventModel> locks)
        {
            HasDataLocks = locks.Any();

            var providerEarnings = Filter(activeApprenticeship, earnings);

            CollectionPeriods = providerEarnings
                .Select(earning => earning.ToCollectionPeriod(activeApprenticeship, locks))
                .GroupBy(x => x.Period)
                .Select(x => x.First())
                .OrderByDescending(x => x)
                .ToList();
        }

        private IEnumerable<EarningEventModel> Filter(
            ApprenticeshipModel activeApprenticeship,
            IEnumerable<EarningEventModel> earnings)
        {
            var predicate = AllEarningsAreForSameProvider(earnings)
                ? IncludeEverythingPredicate
                : IncludeApprenticeshipPredicate(activeApprenticeship);
            return earnings.Where(predicate);
        }

        private static bool AllEarningsAreForSameProvider(IEnumerable<EarningEventModel> earnings) =>
            earnings.GroupBy(x => x.Ukprn).Count() == 1;

        private Func<EarningEventModel, bool> IncludeApprenticeshipPredicate(ApprenticeshipModel apprenticeship)
            => earning => earning.Ukprn == apprenticeship?.Ukprn;

        private static bool IncludeEverythingPredicate(EarningEventModel _) => true;
    }
}