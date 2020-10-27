using SFA.DAS.IdentifyDataLocks.Domain;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.IdentifyDataLocks.UnitTests.TestBuilders
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
        public bool IncludeFunctionalSkills { get; private set; }

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

        internal ApprenticeshipBuilder WithFunctionalSkills() =>
            this.With(x => x.IncludeFunctionalSkills = true);

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
                    ApprenticeshipPriceEpisodes = Episodes.ToApprenticeshipModel(),
                }
            }.ToList();
        }

        private List<EarningEventModel> Earnings => LazyEarnings ??= BuildEarnings();

        private List<EarningEventModel>? LazyEarnings;

        private List<EarningEventModel> BuildEarnings()
        {
            return Enumerable.Range(0, Episodes.NumberOfEarningPeriods)
                .SelectMany(MakeEarnings).ToList();

            IEnumerable<EarningEventModel> MakeEarnings(int month)
            {
                if (IncludeFunctionalSkills)
                {
                    var earning = MakeEarning(month, "SomeOtherFunctionalAim");
                    earning.PriceEpisodes.Clear();
                    yield return earning;
                }

                yield return MakeEarning(month, "ZPROG001");
            }

            EarningEventModel MakeEarning(int month, string aimReference)
            {
                return new EarningEventModel
                {
                    EventId = Guid.NewGuid(),
                    Ukprn = LockedUkprn ?? Ukprn,
                    LearnerUln = IlrUln,
                    AcademicYear = (short)new AcademicYear(Episodes.StartDate.AddMonths(month)),
                    LearningAimReference = aimReference,
                    LearningAimStandardCode = LockedProgramme?.standard ?? StandardCode,
                    LearningAimFrameworkCode = FrameworkCode,
                    LearningAimProgrammeType = ProgrammeType,
                    LearningAimPathwayCode = PathwayCode,
                    PriceEpisodes = Episodes.ToEarningsModel(),
                };
            }
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
        public class Data
        {
            public int TotalNegotiatedPrice1 { get; set; } = 100;
            public int TotalNegotiatedPrice2 { get; set; } = 200;
            public int TotalNegotiatedPrice3 { get; set; } = 300;
            public int TotalNegotiatedPrice4 { get; set; } = 400;
            public DateTime StartDate { get; set; } = new DateTime(2020, 03, 15);
            public DateTime? StoppedDate { get; set; }
            public int NumberOfEarningPeriods { get; set; } = 3;
            public decimal Cost { get; set; }
        }

        private readonly List<Data> Episodes = new List<Data> { new Data() };

        private Data CurrentEpisode => Episodes.Last();

        public DateTime StartDate => CurrentEpisode.StartDate;
        public DateTime? StoppedDate => CurrentEpisode.StoppedDate;
        public int NumberOfEarningPeriods => CurrentEpisode.NumberOfEarningPeriods;

        internal ApprenticePriceEpisodeBuilder WithPriceFromTnp(int tnp1, int tnp2, int tnp3, int tnp4)
            => this.With(x =>
            {
                x.CurrentEpisode.TotalNegotiatedPrice1 = tnp1;
                x.CurrentEpisode.TotalNegotiatedPrice2 = tnp2;
                x.CurrentEpisode.TotalNegotiatedPrice3 = tnp3;
                x.CurrentEpisode.TotalNegotiatedPrice4 = tnp4;
                x.CurrentEpisode.Cost = tnp3 + tnp4;
            });

        internal ApprenticePriceEpisodeBuilder WithPriceFromTnp1And2(int tnp1, int tnp2)
            => this.With(x =>
            {
                x.CurrentEpisode.TotalNegotiatedPrice1 = tnp1;
                x.CurrentEpisode.TotalNegotiatedPrice2 = tnp2;
                x.CurrentEpisode.TotalNegotiatedPrice3 = 0;
                x.CurrentEpisode.TotalNegotiatedPrice4 = 0;
                x.CurrentEpisode.Cost = tnp1 + tnp2;
            });

        internal ApprenticePriceEpisodeBuilder WithPriceFromTnp3And4(int tnp3, int tnp4)
            => WithPriceFromTnp(0, 0, tnp3, tnp4);

        internal ApprenticePriceEpisodeBuilder Starting(DateTime starting) =>
            this.With(x =>
            {
                x.CurrentEpisode.StartDate = starting;
            });

        internal ApprenticePriceEpisodeBuilder WithEarnings(int numMonths) =>
            this.With(x => x.CurrentEpisode.NumberOfEarningPeriods = numMonths);

        internal ApprenticePriceEpisodeBuilder Stopped(DateTime stopped) =>
            this.With(x =>
            {
                x.CurrentEpisode.StoppedDate = stopped;
            });

        internal ApprenticePriceEpisodeBuilder Configure(
            Func<ApprenticePriceEpisodeBuilder, ApprenticePriceEpisodeBuilder>? configure)
            => configure?.Invoke(this) ?? this;

        internal ApprenticePriceEpisodeBuilder AddPriceEpisode()
        {
            return this.With(x =>
            {
            });
        }

        internal List<EarningEventPriceEpisodeModel> ToEarningsModel()
        {
            return new List<EarningEventPriceEpisodeModel>
            {
                new EarningEventPriceEpisodeModel
                {
                    StartDate = StartDate,
                    ActualEndDate = StoppedDate,
                    TotalNegotiatedPrice1 = CurrentEpisode.TotalNegotiatedPrice1,
                    TotalNegotiatedPrice2 = CurrentEpisode.TotalNegotiatedPrice2,
                    TotalNegotiatedPrice3 = CurrentEpisode.TotalNegotiatedPrice3,
                    TotalNegotiatedPrice4 = CurrentEpisode.TotalNegotiatedPrice4,
                }
            };
        }

        internal List<ApprenticeshipPriceEpisodeModel> ToApprenticeshipModel()
        {
            return new List<ApprenticeshipPriceEpisodeModel>
            {
                new ApprenticeshipPriceEpisodeModel
                {
                    StartDate = StartDate,
                    EndDate = StoppedDate,
                    Cost = CurrentEpisode.Cost,
                }
            };
        }
    }
}