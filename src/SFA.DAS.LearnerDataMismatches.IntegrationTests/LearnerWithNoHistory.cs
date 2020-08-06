using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SFA.DAS.LearnerDataMismatches.Web.Pages;
using SFA.DAS.Payments.Model.Core.Audit;
using System.Threading.Tasks;

namespace SFA.DAS.LearnerDataMismatches.IntegrationTests
{
    public class LearnerWithNoHistory
    {
        [SetUp]
        public Task SetUp() => Testing.Reset();

        [Test]
        public async Task Then_there_are_no_CollectionPeriods()
        {
            var learner = Testing.Create<LearnerModel>();
            await learner.OnGetAsync();

            learner.CollectionPeriods.Should().BeEmpty();
        }

        [Test]
        public async Task Insert_some_history()
        {
            var earning = new EarningEventModel
            {
                Ukprn = 12345678,
                LearnerUln = 8888888,
            };

            await Testing.AddAsync(earning);

            var learner = Testing.Create<LearnerModel>();
            learner.Uln = "8888888";
            await learner.OnGetAsync();

            learner.NewCollectionPeriods.Should().ContainEquivalentOf(
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
