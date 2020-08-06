using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.LearnerDataMismatches.Web.Pages;
using System.Threading.Tasks;

namespace SFA.DAS.LearnerDataMismatches.IntegrationTests
{
    public class LearnerWithNoHistory
    {
        [Test]
        public async Task Then_there_are_no_CollectionPeriods()
        {
            var learner = Testing.Create<LearnerModel>();
            await learner.OnGetAsync();

            learner.CollectionPeriods.Should().BeEmpty();
        }
    }
}
