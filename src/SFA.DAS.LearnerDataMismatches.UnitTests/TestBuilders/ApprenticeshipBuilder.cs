using SFA.DAS.LearnerDataMismatches.Domain;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.LearnerDataMismatches.UnitTests
{
    public class ApprenticeshipBuilder
    {
        public long IlrUln { get; private set; } = 123456789;
        public long? ApprenticeshipUln { get; private set; } = 123456789;
        public string FirstName { get; private set; } = "Stephen";

        public string LastName { get; private set; } = "Hawking";
        public int StandardCode { get; private set; } = 50;
        public int FrameworkCode { get; private set; } = 25;
        public int ProgrammeType { get; private set; } = 12;
        public int PathwayCode { get; private set; } = 11;

        public ApprenticePriceEpisodeBuilder Episodes { get; private set; }
            = new ApprenticePriceEpisodeBuilder();

        public int Ukprn { get; private set; } = 111222333;
        public string ProviderName { get; private set; } = "Cambridge College";

        private int? LockedUkprn { get; set; }

        private (int? standard, int? framework, int? programme, int? pathway)? LockedProgramme;

        internal ApprenticeshipBuilder ForLearner(
            int uln = 123456789,
            string firstName = "Stephen",
            string lastName = "Hawking",
            int? lockedUln = null) =>
            this.With(x =>
            {
                x.ApprenticeshipUln = x.IlrUln = uln;
                x.FirstName = firstName;
                x.LastName = lastName;
                //x.LockedUln = lockedUln;
            });

        internal ApprenticeshipBuilder ForMissingLearner(
            int uln = 123456789,
            string firstName = "Stephen",
            string lastName = "Hawking")
            => ForLearner(uln, firstName, lastName)
                .With(x => x.ApprenticeshipUln = null);

        internal ApprenticeshipBuilder WithProvider(
            int ukprn = 111222333,
            string name = "Cambridge College",
            int? lockedUkprn = null) =>
            this.With(x =>
            {
                x.Ukprn = ukprn;
                x.ProviderName = name;
                x.LockedUkprn = lockedUkprn;
            });

        internal ApprenticeshipBuilder ForProgramme(
            int standardCode = 50,
            int frameworkCode = 25,
            int programmeType = 12,
            int pathwayCode = 11,
            int? lockedStandardCode = null,
            int? lockedFrameworkCode = null,
            int? lockedProgrammeType = null,
            int? lockedPathwayCode = null,
            Func<ApprenticePriceEpisodeBuilder, ApprenticePriceEpisodeBuilder>? episodes = null) =>
            this.With(x =>
            {
                x.StandardCode = standardCode;
                x.FrameworkCode = frameworkCode;
                x.ProgrammeType = programmeType;
                x.PathwayCode = pathwayCode;
                x.LockedProgramme =
                    (lockedStandardCode
                        , lockedFrameworkCode
                        , lockedProgrammeType
                        , lockedPathwayCode);
                x.Episodes = x.Episodes.Copy().Configure(episodes);
            });

        internal IEnumerable<ApprenticeshipModel> BuildApprentices()
        {
            if (ApprenticeshipUln == null)
                return Enumerable.Empty<ApprenticeshipModel>();

            return new[]
            {
                new ApprenticeshipModel
                {
                    Uln = ApprenticeshipUln.Value,
                    Ukprn = Ukprn,
                    Status = Payments.Model.Core.Entities.ApprenticeshipStatus.Active,
                    StandardCode = StandardCode,
                    FrameworkCode = FrameworkCode,
                    ProgrammeType = ProgrammeType,
                    PathwayCode = PathwayCode,
                    StopDate = Episodes.StoppedDate,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel
                        {
                            StartDate = Episodes.StartDate,
                            EndDate = Episodes.StoppedDate,
                            Cost = Episodes.TotalNegotiatedPrice1 +
                                   Episodes.TotalNegotiatedPrice2 +
                                   Episodes.TotalNegotiatedPrice3 +
                                   Episodes.TotalNegotiatedPrice4,
                        }
                    }
                }
            }.ToList();
        }

        private List<EarningEventModel> Earnings => LazyEarnings ??= BuildEarnings();

        private List<EarningEventModel>? LazyEarnings;

        private List<EarningEventModel> BuildEarnings()
        {
            return Enumerable.Range(0, Episodes.NumberOfEarningPeriods)
                .Select(i => new EarningEventModel
                {
                    EventId = Guid.NewGuid(),
                    Ukprn = LockedUkprn ?? Ukprn,
                    LearnerUln = IlrUln,
                    AcademicYear = (short)new AcademicYear(Episodes.StartDate.AddMonths(i)),
                    LearningAimStandardCode = LockedProgramme?.standard ?? StandardCode,
                    LearningAimFrameworkCode = FrameworkCode,
                    LearningAimProgrammeType = ProgrammeType,
                    LearningAimPathwayCode = PathwayCode,
                    PriceEpisodes = new List<EarningEventPriceEpisodeModel>
                    {
                        new EarningEventPriceEpisodeModel
                        {
                            StartDate = Episodes.StartDate,
                            ActualEndDate = Episodes.StoppedDate,
                            TotalNegotiatedPrice1 = Episodes.TotalNegotiatedPrice1,
                            TotalNegotiatedPrice2 = Episodes.TotalNegotiatedPrice2,
                            TotalNegotiatedPrice3 = Episodes.TotalNegotiatedPrice3,
                            TotalNegotiatedPrice4 = Episodes.TotalNegotiatedPrice4,
                        }
                    }
                }).ToList();
        }

        private IEnumerable<DataLockEventModel> BuildDataLocks()
        {
            var earning = Earnings.FirstOrDefault();

            DataLockEventModel BuildLock(DataLockErrorCode dataLock, Action<DataLockEventModel> update)
            {
                var dlock = new DataLockEventModel
                {
                    Ukprn = Ukprn,
                    EarningEventId = earning?.EventId ?? Guid.Empty,
                    AcademicYear = earning?.AcademicYear ?? default,
                    LearnerUln = IlrUln,
                    LearningAimStandardCode = StandardCode,
                    LearningAimFrameworkCode = FrameworkCode,
                    LearningAimProgrammeType = ProgrammeType,
                    LearningAimPathwayCode = PathwayCode,
                    NonPayablePeriods = new List<DataLockEventNonPayablePeriodModel>
                    {
                        new DataLockEventNonPayablePeriodModel
                        {
                            DataLockEventNonPayablePeriodFailures =
                                new List<DataLockEventNonPayablePeriodFailureModel>
                                {
                                    new DataLockEventNonPayablePeriodFailureModel
                                    {
                                        DataLockFailure = dataLock,
                                    }
                                }
                        }
                    }
                };
                update(dlock);
                return dlock;
            }

            if (LockedUkprn != null)
                yield return BuildLock(DataLockErrorCode.DLOCK_01, dlock => dlock.Ukprn = LockedUkprn.Value);

            if (ApprenticeshipUln == null)
                yield return BuildLock(DataLockErrorCode.DLOCK_02, dlock => dlock.LearnerUln = IlrUln);

            if (LockedProgramme?.standard != null)
                yield return BuildLock(DataLockErrorCode.DLOCK_03, dlock => dlock.LearningAimStandardCode = LockedProgramme.Value.standard.Value);

            if (LockedProgramme?.framework != null)
                yield return BuildLock(DataLockErrorCode.DLOCK_04, dlock => dlock.LearningAimStandardCode = LockedProgramme.Value.framework.Value);

            if (LockedProgramme?.programme != null)
                yield return BuildLock(DataLockErrorCode.DLOCK_05, dlock => dlock.LearningAimProgrammeType = LockedProgramme.Value.programme.Value);
        }

        internal CollectionPeriodReport CreateLearnerReport(
            Func<IEnumerable<DataLockEventModel>, IEnumerable<DataLockEventModel>>? modifyLocks = null
                                                           )
        {
            var apprenticeships = BuildApprentices();

            var locks = BuildDataLocks();
            if (modifyLocks != null) locks = modifyLocks(locks);

            return new CollectionPeriodReport(apprenticeships.FirstOrDefault(), Earnings, locks.ToList());
        }
    }

    public class ApprenticePriceEpisodeBuilder
    {
        public int TotalNegotiatedPrice1 { get; private set; } = 100;
        public int TotalNegotiatedPrice2 { get; private set; } = 200;
        public int TotalNegotiatedPrice3 { get; private set; } = 300;
        public int TotalNegotiatedPrice4 { get; private set; } = 400;
        public DateTime StartDate { get; private set; } = new DateTime(2020, 03, 15);
        public DateTime? StoppedDate { get; private set; }
        public int NumberOfEarningPeriods { get; private set; } = 3;

        internal ApprenticePriceEpisodeBuilder WithPrice(int tnp1, int tnp2, int tnp3, int tnp4)
            => this.With(x =>
            {
                x.TotalNegotiatedPrice1 = tnp1;
                x.TotalNegotiatedPrice2 = tnp2;
                x.TotalNegotiatedPrice3 = tnp3;
                x.TotalNegotiatedPrice4 = tnp4;
            });

        internal ApprenticePriceEpisodeBuilder Starting(DateTime starting) =>
            this.With(x =>
            {
                x.StartDate = starting;
            });

        internal ApprenticePriceEpisodeBuilder WithEarnings(int numMonths) =>
            this.With(x => x.NumberOfEarningPeriods = numMonths);

        internal ApprenticePriceEpisodeBuilder Stopped(DateTime stopped) =>
            this.With(x =>
            {
                x.StoppedDate = stopped;
            });

        internal ApprenticePriceEpisodeBuilder Configure(
            Func<ApprenticePriceEpisodeBuilder, ApprenticePriceEpisodeBuilder>? configure)
            => configure?.Invoke(this) ?? this;
    }
}