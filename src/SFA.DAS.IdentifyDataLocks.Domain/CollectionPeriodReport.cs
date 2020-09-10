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

            var periods = providerEarnings
                .Select(earning => earning.ToCollectionPeriod(activeApprenticeship, locks))
                .GroupBy(x => x.Period)
                ;

            var firstEarningInPeriods = periods
                .Select(x => x.First())
                ;

            CollectionPeriods = firstEarningInPeriods
                .OrderByDescending(x => x)
                .ToList();
        }

        private IEnumerable<EarningEventModel> Filter(
            ApprenticeshipModel activeApprenticeship,
            IEnumerable<EarningEventModel> earnings)
        {
            var filterProviders = AllEarningsAreForSameProvider(earnings)
                ? IncludeEverythingPredicate
                : IncludeApprenticeshipPredicate(activeApprenticeship);

            return earnings
                .Where(FilterInMainAim)
                .Where(filterProviders);
        }

        private static bool AllEarningsAreForSameProvider(IEnumerable<EarningEventModel> earnings) =>
            earnings.GroupBy(x => x.Ukprn).Count() == 1;

        private Func<EarningEventModel, bool> IncludeApprenticeshipPredicate(ApprenticeshipModel apprenticeship)
            => earning => earning.Ukprn == apprenticeship?.Ukprn;

        private static bool IncludeEverythingPredicate(EarningEventModel _) => true;

        private static bool FilterInMainAim(EarningEventModel e) =>
            e.LearningAimReference == "ZPROG001";
    }
}