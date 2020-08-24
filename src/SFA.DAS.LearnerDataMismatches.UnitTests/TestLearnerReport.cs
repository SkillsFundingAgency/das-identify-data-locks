using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.LearnerDataMismatches.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.LearnerDataMismatches.UnitTests
{
    public class TestLearnerReport
    {
        private static IEnumerable<TestCaseData> IndividualPeriodValues()
        {
            static IEnumerable<(string name, Func<ApprenticeshipBuilder> input, object expected)> cases()
            {
                yield return (
                    "UKPRN",
                    () => builder().WithProvider(ukprn: 12),
                    new { Ukprn = 12 });

                yield return (
                    "ULN",
                    () => builder().ForLearner(uln: 12),
                    new { Uln = 12 });

                yield return (
                    "Standard Code",
                    () => builder().ForProgramme(standardCode: 12),
                    new { Standard = 12 });

                yield return (
                    "Framework Code",
                    () => builder().ForProgramme(frameworkCode: 12),
                    new { Framework = 12 });

                yield return (
                    "Programme Type",
                    () => builder().ForProgramme(programmeType: 12),
                    new { Program = 12 });

                yield return (
                    "Pathway Code",
                    () => builder().ForProgramme(pathwayCode: 12),
                    new { Pathway = 12 });

                yield return (
                    "Cost",
                    () => builder().ForProgramme(episodes: episodes =>
                                                 episodes.WithPrice(1, 2, 3, 4)),
                    new { Cost = 10 });

                yield return (
                    () => builder().ForProgramme(episodes: episodes =>
                                                 episodes.Starting(new DateTime(2020, 01, 25))),
                    new { PriceStart = new DateTime(2020, 01, 25) });
            }

            static ApprenticeshipBuilder builder() => new ApprenticeshipBuilder();

            return cases().Select(x => new TestCaseData(x.input, x.expected)
                                               .SetName($"Populates {x.name}"));
        }

        [Test, TestCaseSource(nameof(IndividualPeriodValues))]
        public void PopulatessDataMatches(Func<ApprenticeshipBuilder> build, object period)
        {
            var a = build();

            var sut = a.CreateLearnerReport();

            sut.CollectionPeriods.Should().ContainEquivalentOf(
                new
                {
                    Apprenticeship = period,
                    Ilr = period,
                });
        }

        [Test]
        public void PopulatesDataLock()
        {
            var builder = new ApprenticeshipBuilder()
                .ForProgramme(standardCode: 10, lockedStandardCode: 11);

            var sut = builder.CreateLearnerReport();

            sut.CollectionPeriods.Should().ContainEquivalentOf(
                new
                {
                    DataLocks = new[] { DataLock.Dlock03 },
                });
        }
    }
}