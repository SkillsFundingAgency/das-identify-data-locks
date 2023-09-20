using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.IdentifyDataLocks.Web.Constants;
using SFA.DAS.IdentifyDataLocks.Web.Pages;

namespace SFA.DAS.IdentifyDataLocks.UnitTests.Web.Index
{
    public class WhenIndexOnGet
    {
        [Test, AutoData]
        public void Then_Return_SamePropertyValue(bool useDfESignIn)
        {
            //arrange
            var logger = new Mock<ILogger<IndexModel>>();
            var configuration = new Mock<IConfiguration>();
            var configurationSection = new Mock<IConfigurationSection>();
            configurationSection.Setup(a => a.Value).Returns(useDfESignIn.ToString());
            configuration.Setup(a => a.GetSection(ConfigKey.UseDfESignIn)).Returns(configurationSection.Object);

            //sut
            var model = new IndexModel(logger.Object, configuration.Object);
            model.OnGet();

            //assert
            var actual = model.UseDfESignIn;
            actual.Should().Be(useDfESignIn);
        }
    }
}
