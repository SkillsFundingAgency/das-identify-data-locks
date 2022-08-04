using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.IdentifyDataLocks.Data.Model;
using SFA.DAS.IdentifyDataLocks.Domain.Services;
using SFA.DAS.IdentifyDataLocks.IntegrationTests.Helpers;

namespace SFA.DAS.IdentifyDataLocks.IntegrationTests.LearnerWithStoppedApprenticeship
{
    public class LearnerWithStoppedApprenticeship
    {
        private readonly WebApplicationFactory<TestableStartup> _app;

        private ScopedContext Context { get; }
        private readonly ITimeProvider _timeProvider;

        public LearnerWithStoppedApprenticeship()
        {
            var commitmentsApi = Substitute.For<ICommitmentsApiClient>();
            var roatpApi = Substitute.For<IProviderService>();
            
            var accountsApi = Substitute.For<IAccountApiClient>();
            accountsApi.GetAccount(Arg.Any<long>()).Returns(new AccountDetailViewModel { DasAccountName = "test", PublicHashedAccountId = "test" });

            _timeProvider = Substitute.For<ITimeProvider>();

            _app = new IdentifyDataLocksWebApplicationFactory<TestableStartup>()
                .WithWebHostBuilder(host =>
                {
                    host.ConfigureServices(services =>
                    {
                        services
                            .ConfigureMockService(_ => commitmentsApi)
                            .ConfigureMockService(_ => roatpApi)
                            .ConfigureMockService(_ => accountsApi)
                            .ConfigureMockService(_ => _timeProvider)
                            .ConfigurePaymentsAuditDataContext()
                            .ConfigurePaymentsAuditDataContext();
                    });
                });

            var scopeFactory = _app.Services.GetService<IServiceScopeFactory>();
            Context = new ScopedContext(scopeFactory);
        }

        private async Task Arrange(string prefix)
        {
            var apps = await Context.AddEntitiesFromJsonResource<ApprenticeshipModel>($"{prefix}.Apprenticeships.json");

            if (apps.Length == 0) throw new Exception("There must be an apprenticeship to run these tests.");

            await Context.AddEntitiesFromJsonResource<EarningEventModel>($"{prefix}.EarningEvents.json");

            await Context.AddEntitiesFromJsonResource<DataLockEventModel>($"{prefix}.Datalocks.json");

            _timeProvider.Today.Returns(new DateTime(2019, 08, 01));
        }
        
        [Test]
        public async Task RendersDataMismatches()
        {
            await Arrange("SFA.DAS.IdentifyDataLocks.IntegrationTests.TestData.LearnerWithStoppedApprenticeship");

            var result = await _app.CreateClient().GetAsync("/Learner/6111511121");

            result.EnsureSuccessStatusCode(); // Status Code 200-299

            result.Content.Headers.ContentType.ToString().Should().Be("text/html; charset=utf-8");

            var data = await result.Content.ReadAsStringAsync();

            data.Should().ContainAll("6111511121", "10010010", "10030030");
        }
    }
}