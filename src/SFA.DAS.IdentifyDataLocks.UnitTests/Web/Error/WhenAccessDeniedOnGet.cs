using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using SFA.DAS.IdentifyDataLocks.Web.Constants;
using SFA.DAS.IdentifyDataLocks.Web.Pages;

namespace SFA.DAS.IdentifyDataLocks.UnitTests.Web.Error
{
    public class WhenAccessDeniedOnGet
    {
        [TestCase("test", "https://test-services.signin.education.gov.uk/approvals/select-organisation?action=request-service", true)]
        [TestCase("pp", "https://test-services.signin.education.gov.uk/approvals/select-organisation?action=request-service", true)]
        [TestCase("local", "https://test-services.signin.education.gov.uk/approvals/select-organisation?action=request-service", false)]
        [TestCase("prd", "https://services.signin.education.gov.uk/approvals/select-organisation?action=request-service", false)]
        [TestCase("", "https://services.signin.education.gov.uk/approvals/select-organisation?action=request-service", false)]
        public void Then_Return_SamePropertyValue(string env, string helpLink, bool useDfESignIn)
        {
            //arrange
            var mockConfiguration = new Mock<IConfiguration>();
            var mockDfESignInSection = new Mock<IConfigurationSection>();
            var mockResourceEnvSection = new Mock<IConfigurationSection>();

            mockDfESignInSection.Setup(a => a.Value).Returns(useDfESignIn.ToString());
            mockResourceEnvSection.Setup(a => a.Value).Returns(env);

            mockConfiguration.Setup(a => a.GetSection(ConfigKey.ResourceEnvironmentName)).Returns(mockResourceEnvSection.Object);
            mockConfiguration.Setup(a => a.GetSection(ConfigKey.UseDfESignIn)).Returns(mockDfESignInSection.Object);

            //sut
            var model = new AccessDenied(mockConfiguration.Object);
            model.OnGet();

            //assert
            model.UseDfESignIn.Should().Be(useDfESignIn);
            model.HelpPageLink.Should().Be(helpLink);
        }
    }
}
