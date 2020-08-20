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
            int pathwayCode = 11) =>
            this.With(x =>
            {
                x.StandardCode = standardCode;
                x.FrameworkCode = frameworkCode;
                x.ProgrammeType = programmeType;
                x.PathwayCode = pathwayCode;
            });

        internal ApprenticeshipBuilder WithPriceEpisode(int tnp1, int tnp2, int tnp3, int tnp4)
        {
            return this;
        }

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
                }
            }.ToList();
        }
    }
}