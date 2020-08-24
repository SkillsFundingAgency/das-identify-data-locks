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
        public int Uln { get; private set; } = 123456789;
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

        private int? lockedUkprn { get; set; }
        private (int? standard, int? framework, int? programme, int? pathway)? LockedProgramme;

        internal ApprenticeshipBuilder ForLearner(
            int uln = 123456789,
            string firstName = "Stephen",
            string lastName = "Hawking") =>
            this.With(x =>
            {
                x.Uln = uln;
                x.FirstName = firstName;
                x.LastName = lastName;
            });

        internal ApprenticeshipBuilder WithProvider(
            int ukprn = 111222333,
            string name = "Cambridge College",
            int? locked = null) =>
            this.With(x =>
            {
                x.Ukprn = ukprn;
                x.ProviderName = name;
                x.lockedUkprn = locked;
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
                    ( lockedStandardCode
                    , lockedFrameworkCode
                    , lockedProgrammeType
                    , lockedPathwayCode);
                x.Episodes = x.Episodes.Copy().Configure(episodes);
            });

        internal List<ApprenticeshipModel> BuildApprentices()
        {
            return new[]
            {
                new ApprenticeshipModel
                {
                    Uln = Uln,
                    Ukprn = Ukprn,
                    Status = Payments.Model.Core.Entities.ApprenticeshipStatus.Active,
                    StandardCode = StandardCode,
                    FrameworkCode = FrameworkCode,
                    ProgrammeType = ProgrammeType,
                    PathwayCode = PathwayCode,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel
                        {
                            StartDate = Episodes.StartDate,
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
            return new[]
            {
                new EarningEventModel
                {
                    EventId = Guid.NewGuid(),
                    Ukprn = Ukprn,
                    LearnerUln = Uln,
                    LearningAimStandardCode = LockedProgramme?.standard ?? StandardCode,
                    LearningAimFrameworkCode = FrameworkCode,
                    LearningAimProgrammeType = ProgrammeType,
                    LearningAimPathwayCode = PathwayCode,
                    PriceEpisodes = new List<EarningEventPriceEpisodeModel>
                    {
                        new EarningEventPriceEpisodeModel
                        {
                            StartDate = Episodes.StartDate,
                            TotalNegotiatedPrice1 = Episodes.TotalNegotiatedPrice1,
                            TotalNegotiatedPrice2 = Episodes.TotalNegotiatedPrice2,
                            TotalNegotiatedPrice3 = Episodes.TotalNegotiatedPrice3,
                            TotalNegotiatedPrice4 = Episodes.TotalNegotiatedPrice4,
                        }
                    }
                }
            }.ToList();
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
                    LearnerUln = Uln,
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

            if (lockedUkprn != null)
                yield return BuildLock(DataLockErrorCode.DLOCK_01, dlock => dlock.Ukprn = lockedUkprn.Value);

            if (LockedProgramme?.standard != null)
                yield return BuildLock(DataLockErrorCode.DLOCK_03, dlock => dlock.LearningAimStandardCode = LockedProgramme.Value.standard.Value);

            if (LockedProgramme?.framework != null)
                yield return BuildLock(DataLockErrorCode.DLOCK_04, dlock => dlock.LearningAimStandardCode = LockedProgramme.Value.framework.Value);

            if (LockedProgramme?.programme != null)
                yield return BuildLock(DataLockErrorCode.DLOCK_05, dlock => dlock.LearningAimProgrammeType = LockedProgramme.Value.programme.Value);
        }

        internal LearnerReport CreateLearnerReport()
        {
            var value = Earnings;
            return new LearnerReport(BuildApprentices().FirstOrDefault(), value, BuildDataLocks().ToList());
        }
    }

    public class ApprenticePriceEpisodeBuilder
    {
        public int TotalNegotiatedPrice1 { get; private set; } = 100;
        public int TotalNegotiatedPrice2 { get; private set; } = 200;
        public int TotalNegotiatedPrice3 { get; private set; } = 300;
        public int TotalNegotiatedPrice4 { get; private set; } = 400;
        public DateTime StartDate { get; private set; } = new DateTime(2020, 03, 15);

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

        internal ApprenticePriceEpisodeBuilder Configure(
            Func<ApprenticePriceEpisodeBuilder, ApprenticePriceEpisodeBuilder>? configure)
            => configure?.Invoke(this) ?? this;
    }
}