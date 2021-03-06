﻿using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.IdentifyDataLocks.Domain;
using SFA.DAS.IdentifyDataLocks.UnitTests.TestBuilders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.IdentifyDataLocks.UnitTests
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
                    "Cost from TNP1/2",
                    () => builder().ForProgramme(episodes: episodes =>
                                                 episodes.WithPriceFromTnp1And2(1, 2)),
                    new { Cost = 3 });

                yield return (
                    "Cost from TNP3/4",
                    () => builder().ForProgramme(episodes: episodes =>
                                                 episodes.WithPriceFromTnp3And4(3, 4)),
                    new { Cost = 7 });

                yield return (
                    "Cost from TNP3/4 (ignores TNP1/2)",
                    () => builder().ForProgramme(episodes: episodes =>
                                                 episodes.WithPriceFromTnp(1, 2, 3, 4)),
                    new { Cost = 7 });

                yield return (
                    "Starting",
                    () => builder().ForProgramme(episodes: episodes =>
                                                 episodes.Starting(new DateTime(2020, 01, 25))),
                    new { PriceStart = new DateTime(2020, 01, 25) });

                yield return (
                    "Stopped",
                    () => builder().ForProgramme(episodes: episodes =>
                                                 episodes.Stopped(new DateTime(2020, 01, 26))),
                    new { StoppedOn = new DateTime(2020, 01, 26) });
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

        [Test]
        public void PopulatesDataLocksInOrder()
        {
            var builder = new ApprenticeshipBuilder()
                .ForProgramme(standardCode: 10, lockedStandardCode: 11,
                              frameworkCode: 20, lockedFrameworkCode: 22);

            var sut = builder.CreateLearnerReport(modifyLocks: l =>
                l.OrderByDescending(x => x.NonPayablePeriods
                    .SelectMany(x => x.DataLockEventNonPayablePeriodFailures)
                    .Select(x => x.DataLockFailure)
                    .First()));

            foreach (var p in sut.CollectionPeriods)
                p.DataLocks.Should().HaveCountGreaterOrEqualTo(2).And.BeInAscendingOrder(x => x);
        }

        [Test]
        public void PopulatesDataLocksWithoutDuplicates()
        {
            var builder = new ApprenticeshipBuilder()
                .ForProgramme(standardCode: 10, lockedStandardCode: 11);

            var sut = builder.CreateLearnerReport(modifyLocks: l => l.Append(l.First()));

            foreach (var p in sut.CollectionPeriods)
                p.DataLocks.Should().OnlyHaveUniqueItems();
        }

        [Test]
        public void GroupsCollectionPeriodsByAcademicYear()
        {
            var builder = new ApprenticeshipBuilder()
                .ForProgramme(episodes: e => e.WithEarnings(numMonths: 15));

            var sut = builder.CreateLearnerReport();

            sut.CollectionPeriodsByYear.Should().ContainKeys(1920, 2021);
        }

        [Test]
        public void Populates_Cost_From_Tnp_1_And_2()
        {
            var builder = new ApprenticeshipBuilder()
                .ForProgramme(episodes: episodes => episodes
                    .WithPriceFromTnp(1, 2, 3, 4)
                    .Starting(new DateTime(2020, 03, 15))
                    .AddPriceEpisode()
                    .WithPriceFromTnp1And2(5, 6)
                    .Starting(new DateTime(2020, 03, 20))
                    );

            var sut = builder.CreateLearnerReport();

            sut.CollectionPeriods.Should().ContainEquivalentOf(new
            {
                Ilr = new
                {
                    Tnp1 = new[]
                    {
                        new AmountFromDate(new DateTime(2020, 03, 15), 1),
                        new AmountFromDate(new DateTime(2020, 03, 20), 5),
                    },
                    Tnp2 = new[]
                    {
                        new AmountFromDate(new DateTime(2020, 03, 15), 2),
                        new AmountFromDate(new DateTime(2020, 03, 20), 6),
                    },
                    Tnp3 = new[]
                    {
                        new AmountFromDate(new DateTime(2020, 03, 15), 3),
                        new AmountFromDate(new DateTime(2020, 03, 20), 0),
                    },
                    Tnp4 = new[]
                    {
                        new AmountFromDate(new DateTime(2020, 03, 15), 4),
                        new AmountFromDate(new DateTime(2020, 03, 20), 0),
                    },
                }
            });
        }
    }
}