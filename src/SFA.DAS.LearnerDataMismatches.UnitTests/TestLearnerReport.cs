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
            static IEnumerable<(Func<ApprenticeshipBuilder> input, object expected)> cases()
            {
                yield return (
                    () => builder().WithProvider(ukprn: 12),
                    new { Ukprn = 12 });

                yield return (
                    () => builder().ForLearner(uln: 12),
                    new { Uln = 12 });

                yield return (
                    () => builder().ForProgramme(standardCode: 12),
                    new { Standard = 12 });

                yield return (
                    () => builder().ForProgramme(frameworkCode: 12),
                    new { Framework = 12 });

                yield return (
                    () => builder().ForProgramme(programmeType: 12),
                    new { Program = 12 });

                yield return (
                    () => builder().ForProgramme(pathwayCode: 12),
                    new { Pathway = 12 });

                yield return (
                    () => builder().ForProgramme(episodes: episodes =>
                                                 episodes.WithPrice(1, 2, 3, 4)),
                    new { Cost = 10 });
            }

            static ApprenticeshipBuilder builder() => new ApprenticeshipBuilder();

            return cases().Select(x => new TestCaseData(x.input, x.expected));
        }

        [Test, TestCaseSource(nameof(IndividualPeriodValues))]
        public void B(Func<ApprenticeshipBuilder> build, object period)
        {
            var a = build();

            var sut = new LearnerReport(a.BuildApprentices(), a.BuildEarnings());

            sut.CollectionPeriods.Should().ContainEquivalentOf(
                new
                {
                    Apprenticeship = period,
                    Ilr = period,
                });
        }
    }
}