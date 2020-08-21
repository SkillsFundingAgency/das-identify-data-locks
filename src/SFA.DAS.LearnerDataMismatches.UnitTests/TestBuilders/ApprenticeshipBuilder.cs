#nullable enable

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
            string name = "Cambridge College") =>
            this.With(x =>
            {
                x.Ukprn = ukprn;
                x.ProviderName = name;
            });

        internal ApprenticeshipBuilder ForProgramme(
            int standardCode = 50,
            int frameworkCode = 25,
            int programmeType = 12,
            int pathwayCode = 11,
            Func<ApprenticePriceEpisodeBuilder, ApprenticePriceEpisodeBuilder>? episodes = null) =>
            this.With(x =>
            {
                x.StandardCode = standardCode;
                x.FrameworkCode = frameworkCode;
                x.ProgrammeType = programmeType;
                x.PathwayCode = pathwayCode;
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
                    Status = ApprenticeshipStatus.Active,
                    StandardCode = StandardCode,
                    FrameworkCode = FrameworkCode,
                    ProgrammeType = ProgrammeType,
                    PathwayCode = PathwayCode,
                    ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>
                    {
                        new ApprenticeshipPriceEpisodeModel
                        {
                            Cost = Episodes.TotalNegotiatedPrice1 +
                                   Episodes.TotalNegotiatedPrice2 +
                                   Episodes.TotalNegotiatedPrice3 +
                                   Episodes.TotalNegotiatedPrice4,
                        }
                    }
                }
            }.ToList();
        }

        internal List<EarningEventModel> BuildEarnings()
        {
            return new[]
            {
                new EarningEventModel
                {
                    Ukprn = Ukprn,
                    LearnerUln = Uln,
                    LearningAimStandardCode = StandardCode,
                    LearningAimFrameworkCode = FrameworkCode,
                    LearningAimProgrammeType = ProgrammeType,
                    LearningAimPathwayCode = PathwayCode,
                    PriceEpisodes = new List<EarningEventPriceEpisodeModel>
                    {
                        new EarningEventPriceEpisodeModel
                        {
                            TotalNegotiatedPrice1 = Episodes.TotalNegotiatedPrice1,
                            TotalNegotiatedPrice2 = Episodes.TotalNegotiatedPrice2,
                            TotalNegotiatedPrice3 = Episodes.TotalNegotiatedPrice3,
                            TotalNegotiatedPrice4 = Episodes.TotalNegotiatedPrice4,
                        }
                    }
                }
            }.ToList();
        }
    }

    public class ApprenticePriceEpisodeBuilder
    {
        public int TotalNegotiatedPrice1 { get; private set; } = 100;
        public int TotalNegotiatedPrice2 { get; private set; } = 200;
        public int TotalNegotiatedPrice3 { get; private set; } = 300;
        public int TotalNegotiatedPrice4 { get; private set; } = 400;

        internal ApprenticePriceEpisodeBuilder WithPrice(int tnp1, int tnp2, int tnp3, int tnp4)
            => this.With(x =>
            {
                x.TotalNegotiatedPrice1 = tnp1;
                x.TotalNegotiatedPrice2 = tnp2;
                x.TotalNegotiatedPrice3 = tnp3;
                x.TotalNegotiatedPrice4 = tnp4;
            });

        internal ApprenticePriceEpisodeBuilder Configure(
            Func<ApprenticePriceEpisodeBuilder, ApprenticePriceEpisodeBuilder>? configure)
            => configure?.Invoke(this) ?? this;
    }
}