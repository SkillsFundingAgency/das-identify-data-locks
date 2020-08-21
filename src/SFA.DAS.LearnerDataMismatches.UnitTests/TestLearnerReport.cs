using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.LearnerDataMismatches.Domain;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;
using System.Linq;

namespace SFA.DAS.LearnerDataMismatches.UnitTests
{
    public class TestLearnerReport
    {
        [Test]
        public void A()
        {
            var apprentices = new[]
            {
                new ApprenticeshipModel
                {
                    Ukprn = 12345678,
                    Uln = 8888888,
                    Status = Payments.Model.Core.Entities.ApprenticeshipStatus.Active,
                    StandardCode = 0,
                    FrameworkCode = 0,
                    ProgrammeType = 0,
                    PathwayCode = 0,
                }
            }.ToList();

            var earning = new[]
            {
                new EarningEventModel
                {
                    Ukprn = 12345678,
                    LearnerUln = 8888888,
                }
            }.ToList();

            var sut = new LearnerReport(apprentices, earning);

            sut.CollectionPeriods.Should().ContainEquivalentOf(
                new
                {
                    Ilr = new
                    {
                        Ukprn = 12345678,
                    }
                });
        }
    }
}