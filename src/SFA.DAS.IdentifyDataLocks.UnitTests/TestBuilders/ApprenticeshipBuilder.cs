using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.IdentifyDataLocks.Data.Model;
using SFA.DAS.IdentifyDataLocks.Domain;
using SFA.DAS.IdentifyDataLocks.UnitTests.Utilities;

namespace SFA.DAS.IdentifyDataLocks.UnitTests.TestBuilders
{
    public class ApprenticeshipBuilder
    {
        private long IlrUln { get; set; } = 123456789;
        private long? ApprenticeshipUln { get; set; } = 123456789;
        private string FirstName { get; set; } = "Stephen";

        private string LastName { get; set; } = "Hawking";
        private short? StandardCode { get; set; } = 50;
        private short? FrameworkCode { get; set; } = 25;
        private short? ProgrammeType { get; set; } = 12;
        private short? PathwayCode { get; set; } = 11;
        private bool IncludeFunctionalSkills { get; set; }

        private ApprenticePriceEpisodeBuilder Episodes { get; set; } = new ApprenticePriceEpisodeBuilder();

        private int Ukprn { get; set; } = 111222333;

        private string ProviderName { get; set; } = "Cambridge College";

        private int? LockedUkprn { get; set; }

        private (short? standard, short? framework, short? programme, short? pathway)? _lockedProgramme;

        internal ApprenticeshipBuilder ForLearner(
            int uln = 123456789,
            string firstName = "Stephen",
            string lastName = "Hawking")
        {
            return this.With(x =>
            {
                x.ApprenticeshipUln = x.IlrUln = uln;
                x.FirstName = firstName;
                x.LastName = lastName;
                //x.LockedUln = lockedUln;
            });
        }

        internal ApprenticeshipBuilder ForMissingLearner(
            int uln = 123456789,
            string firstName = "Stephen",
            string lastName = "Hawking")
        {
            return ForLearner(uln, firstName, lastName)
                .With(x => x.ApprenticeshipUln = null);
        }

        internal ApprenticeshipBuilder WithProvider(
            int ukprn = 111222333,
            string name = "Cambridge College",
            int? lockedUkprn = null)
        {
            return this.With(x =>
            {
                x.Ukprn = ukprn;
                x.ProviderName = name;
                x.LockedUkprn = lockedUkprn;
            });
        }

        internal ApprenticeshipBuilder ForProgramme(
            short? standardCode = 50,
            short? frameworkCode = 25,
            short? programmeType = 12,
            short? pathwayCode = 11,
            short? lockedStandardCode = null,
            short? lockedFrameworkCode = null,
            short? lockedProgrammeType = null,
            short? lockedPathwayCode = null,
            Func<ApprenticePriceEpisodeBuilder, ApprenticePriceEpisodeBuilder> episodes = null)
        {
            return this.With(x =>
            {
                x.StandardCode = standardCode;
                x.FrameworkCode = frameworkCode;
                x.ProgrammeType = programmeType;
                x.PathwayCode = pathwayCode;
                x._lockedProgramme =
                    (lockedStandardCode
                        , lockedFrameworkCode
                        , lockedProgrammeType
                        , lockedPathwayCode);
                x.Episodes = x.Episodes.Copy().Configure(episodes);
            });
        }

        internal ApprenticeshipBuilder WithFunctionalSkills()
        {
            return this.With(x => x.IncludeFunctionalSkills = true);
        }

        private IEnumerable<ApprenticeshipModel> BuildApprentices()
        {
            if (ApprenticeshipUln == null)
                return Enumerable.Empty<ApprenticeshipModel>();

            return new[]
            {
                new ApprenticeshipModel
                {
                    Uln = ApprenticeshipUln.Value,
                    Ukprn = Ukprn,
                    Status = ApprenticeshipStatus.Active,
                    StandardCode = StandardCode,
                    FrameworkCode = FrameworkCode,
                    ProgrammeType = ProgrammeType,
                    PathwayCode = PathwayCode,
                    StopDate = Episodes.StoppedDate,
                    ApprenticeshipPriceEpisodes = Episodes.ToApprenticeshipModel(),
                }
            }.ToList();
        }

        private List<EarningEventModel> Earnings
        {
            get { return _lazyEarnings ??= BuildEarnings(); }
        }

        private List<EarningEventModel> _lazyEarnings;

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
                    LearningAimStandardCode = _lockedProgramme?.standard ?? StandardCode,
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
                    //EarningEventId = earning?.EventId ?? Guid.Empty,
                    AcademicYear = earning?.AcademicYear ?? default,
                    LearnerUln = IlrUln,
                    //LearningAimStandardCode =  (int)StandardCode,
                    //LearningAimFrameworkCode = (int)FrameworkCode,
                    //LearningAimProgrammeType = (int)ProgrammeType,
                    //LearningAimPathwayCode =   (int)PathwayCode,
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
                yield return BuildLock(DataLockErrorCode.Dlock01, dlock => dlock.Ukprn = LockedUkprn.Value);

            if (ApprenticeshipUln == null)
                yield return BuildLock(DataLockErrorCode.Dlock02, dlock => dlock.LearnerUln = IlrUln);

            if (_lockedProgramme?.standard != null)
                yield return BuildLock(DataLockErrorCode.Dlock03, dlock => { });

            if (_lockedProgramme?.framework != null)
                yield return BuildLock(DataLockErrorCode.Dlock04, dlock => { });

            if (_lockedProgramme?.programme != null)
                yield return BuildLock(DataLockErrorCode.Dlock05, dlock => { });
        }

        internal LearnerReport CreateLearnerReport(Func<IEnumerable<DataLockEventModel>, IEnumerable<DataLockEventModel>> modifyLocks = null)
        {
            var apprenticeships = BuildApprentices();

            var locks = BuildDataLocks();

            if (modifyLocks != null) locks = modifyLocks(locks);

            var dataLockFailures = locks.Select(d => new DataLockFailureModel
            {
                Ukprn = d.Ukprn,
                CollectionPeriod = d.CollectionPeriod,
                AcademicYear = d.AcademicYear,
                DataLockFailures = d.NonPayablePeriods
                    .SelectMany(f => f.DataLockEventNonPayablePeriodFailures)
                    .Select(fe => fe.DataLockFailure)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList()
            }).ToList();

            return new LearnerReport(apprenticeships.FirstOrDefault(), Earnings, dataLockFailures, (new AcademicYear(DateTime.Today), new AcademicYear(DateTime.Today) - 1));
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

        private readonly List<Data> _episodes = new List<Data> { new Data() };

        private Data CurrentEpisode => _episodes.Last();

        public DateTime StartDate => CurrentEpisode.StartDate;

        public DateTime? StoppedDate => CurrentEpisode.StoppedDate;

        public int NumberOfEarningPeriods => CurrentEpisode.NumberOfEarningPeriods;

        internal ApprenticePriceEpisodeBuilder WithPriceFromTnp(int tnp1, int tnp2, int tnp3, int tnp4)
        {
            return this.With(x =>
            {
                x.CurrentEpisode.TotalNegotiatedPrice1 = tnp1;
                x.CurrentEpisode.TotalNegotiatedPrice2 = tnp2;
                x.CurrentEpisode.TotalNegotiatedPrice3 = tnp3;
                x.CurrentEpisode.TotalNegotiatedPrice4 = tnp4;
                x.CurrentEpisode.Cost = tnp3 + tnp4;
            });
        }

        internal ApprenticePriceEpisodeBuilder WithPriceFromTnp1And2(int tnp1, int tnp2)
        {
            return this.With(x =>
            {
                x.CurrentEpisode.TotalNegotiatedPrice1 = tnp1;
                x.CurrentEpisode.TotalNegotiatedPrice2 = tnp2;
                x.CurrentEpisode.TotalNegotiatedPrice3 = 0;
                x.CurrentEpisode.TotalNegotiatedPrice4 = 0;
                x.CurrentEpisode.Cost = tnp1 + tnp2;
            });
        }

        internal ApprenticePriceEpisodeBuilder WithPriceFromTnp3And4(int tnp3, int tnp4)
        {
            return WithPriceFromTnp(0, 0, tnp3, tnp4);
        }

        internal ApprenticePriceEpisodeBuilder Starting(DateTime starting)
        {
            return this.With(x => { x.CurrentEpisode.StartDate = starting; });
        }

        internal ApprenticePriceEpisodeBuilder WithEarnings(int numMonths)
        {
            return this.With(x => x.CurrentEpisode.NumberOfEarningPeriods = numMonths);
        }

        internal ApprenticePriceEpisodeBuilder Stopped(DateTime stopped)
        {
            return this.With(x => x.CurrentEpisode.StoppedDate = stopped);
        }

        internal ApprenticePriceEpisodeBuilder AddPriceEpisode()
        {
            return this.With(x => x._episodes.Add(new Data()));
        }

        internal ApprenticePriceEpisodeBuilder Configure(Func<ApprenticePriceEpisodeBuilder, ApprenticePriceEpisodeBuilder> configure)
        {
            return configure?.Invoke(this) ?? this;
        }

        internal List<EarningEventPriceEpisodeModel> ToEarningsModel()
        {
            return _episodes.Select(x => new EarningEventPriceEpisodeModel
            {
                StartDate = x.StartDate,
                ActualEndDate = x.StoppedDate,
                TotalNegotiatedPrice1 = x.TotalNegotiatedPrice1,
                TotalNegotiatedPrice2 = x.TotalNegotiatedPrice2,
                TotalNegotiatedPrice3 = x.TotalNegotiatedPrice3,
                TotalNegotiatedPrice4 = x.TotalNegotiatedPrice4,
            }).ToList();
        }

        internal List<ApprenticeshipPriceEpisodeModel> ToApprenticeshipModel()
        {
            return _episodes.Select(x => new ApprenticeshipPriceEpisodeModel
            {
                StartDate = x.StartDate,
                EndDate = x.StoppedDate,
                Cost = x.Cost,
            }).ToList();
        }
    }
}