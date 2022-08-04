using System.Collections.Generic;
using System.Linq;
using SFA.DAS.IdentifyDataLocks.Data.Model;

namespace SFA.DAS.IdentifyDataLocks.Domain
{
    public class LearnerReport
    {
        public (string Id, string Name) Learner { get; set; }
        public (string Id, string Name) Employer { get; set; }
        public (string Id, string Name) Provider { get; set; }
        public bool HasDataLocks { get; set; }
        public bool HasMultipleProviders { get; set; }
        public List<CollectionPeriod> CollectionPeriods { get; set; }
        public IEnumerable<CollectionPeriod> CurrentYearDataLocks { get; set; }
        public IEnumerable<CollectionPeriod> PreviousYearDataLocks { get; set; }

        public LearnerReport(ApprenticeshipModel activeApprenticeship, IList<EarningEventModel> earningsEvents, IList<DataLockFailureModel> dataLockEvents, (AcademicYear current, AcademicYear previous) academicYears)
        {
            var providerCount = earningsEvents.GroupBy(x => x.Ukprn).Count();

            HasDataLocks = dataLockEvents.Any();

            HasMultipleProviders = providerCount > 1;

            CollectionPeriods = earningsEvents
                .Where(e => e.LearningAimReference == "ZPROG001")
                .Where(e => providerCount == 1 || e.Ukprn == activeApprenticeship?.Ukprn)
                //Select All ZPROG001 Earnings if single ukprn or filter by activeApprenticeship Ukprn
                .GroupBy(e => new { e.AcademicYear, e.CollectionPeriod })
                .Select(g => g.First())
                //Group Earnings by AcademicYear, CollectionPeriod and select first item from each grouping
                .Select(ge => ge.ToCollectionPeriod(activeApprenticeship, dataLockEvents))
                //convert final selected earnings to view model
                .ToList();

            CurrentYearDataLocks = CollectionPeriods.Where(c => (AcademicYear)c.Period.Year == academicYears.current).OrderByDescending(x => x.Period).ToList();
            PreviousYearDataLocks = CollectionPeriods.Where(c => (AcademicYear)c.Period.Year == academicYears.previous).OrderByDescending(x => x.Period).ToList();
        }
    }
}