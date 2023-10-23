using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.IdentifyDataLocks.Web.Constants;
using SFA.DAS.IdentifyDataLocks.Web.Pages;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace SFA.DAS.IdentifyDataLocks.UnitTests.Web.Index
{
    public class WhenIndexOnGet
    {
        [Test, AutoData]
        public void Then_User_Not_Authenticated_Return_Page(bool useDfESignIn)
        {
            //arrange
            var modelState = new ModelStateDictionary();
            var actionContext = new ActionContext(new DefaultHttpContext()
            {
                User = null
            }, new RouteData(), new PageActionDescriptor(), modelState);
            var modelMetadataProvider = new EmptyModelMetadataProvider();
            var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);
            var pageContext = new PageContext(actionContext)
            {
                ViewData = viewData
            };

            var logger = new Mock<ILogger<IndexModel>>();
            var configuration = new Mock<IConfiguration>();
            var configurationSection = new Mock<IConfigurationSection>();
            configurationSection.Setup(a => a.Value).Returns(useDfESignIn.ToString());
            configuration.Setup(a => a.GetSection(ConfigKey.UseDfESignIn)).Returns(configurationSection.Object);

            //sut
            var model = new IndexModel(logger.Object, configuration.Object)
            {
                PageContext = pageContext
            };
            model.OnGet();

            //assert
            var actual = model.UseDfESignIn;
            actual.Should().Be(useDfESignIn);
        }

        [Test, AutoData]
        public void Then_User_Authenticated_Return_Redirect()
        {
            //arrange
            var httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new GenericIdentity("displayName"))
            };
            var modelState = new ModelStateDictionary();
            var actionContext = new ActionContext(httpContext, new RouteData(), new PageActionDescriptor(), modelState);
            var modelMetadataProvider = new EmptyModelMetadataProvider();
            var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);
            var pageContext = new PageContext(actionContext)
            {
                ViewData = viewData
            };

            var logger = new Mock<ILogger<IndexModel>>();
            var configuration = new Mock<IConfiguration>();

            //sut
            var model = new IndexModel(logger.Object, configuration.Object)
            {
                PageContext = pageContext
            };
            var actual = model.OnGet() as RedirectToPageResult;

            //assert
            actual.Should().NotBeNull();
            actual?.PageName.Should().Be("Start");
        }
    }
}
