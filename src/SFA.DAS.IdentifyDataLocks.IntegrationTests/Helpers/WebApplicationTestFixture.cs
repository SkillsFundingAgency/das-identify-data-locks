using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using Respawn;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.IdentifyDataLocks.IntegrationTests.Helpers;
using SFA.DAS.IdentifyDataLocks.Web.Infrastructure;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Providers.Api.Client;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using PaymentsApprenticeshipStatus = SFA.DAS.Payments.Model.Core.Entities.ApprenticeshipStatus;

namespace SFA.DAS.IdentifyDataLocks.IntegrationTests
{
    [TestFixture]
    public class WebApplicationTestFixture
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly WebApplicationFactory<TestableStartup> app;

        public ScopedContext Context { get; }
        public ApprenticeshipModel Apprenticeship { get; private set; }

        public ICommitmentsApiClient CommitmentsApi;
        public IProviderApiClient ProviderApi;
        public IAccountApiClient AccountsApi;
        public ITimeProvider TimeProvider;

        public WebApplicationTestFixture()
        {
            app = new IdentifyDataLocksWebApplicationFactory<TestableStartup>()
                .WithWebHostBuilder(host =>
                {
                    host.ConfigureServices(services =>
                    {
                        services
                            .ConfigureMockService(_ => CommitmentsApi)
                            .ConfigureMockService(_ => ProviderApi)
                            .ConfigureMockService(_ => AccountsApi)
                            .ConfigureMockService(_ => TimeProvider);
                    });
                });

            scopeFactory = app.Services.GetService<IServiceScopeFactory>();
            Context = new ScopedContext(scopeFactory);
        }

        [SetUp]
        public async Task Reset()
        {
            await Context.Reset();
            CommitmentsApi = Substitute.For<ICommitmentsApiClient>();
            ProviderApi = Substitute.For<IProviderApiClient>();
            AccountsApi = Substitute.For<IAccountApiClient>();
            TimeProvider = Substitute.For<ITimeProvider>();
        }

        protected async Task Arrange(string prefix)
        {
            var apprenticeshipResourceName = $"{prefix}.Apprenticeships.json";

            var apps = await Context.AddEntitiesFromJsonResource<ApprenticeshipModel>($"{prefix}.Apprenticeships.json");
            if (apps.Length == 0) throw new Exception("There must be an apprenticeship to run these tests.");

            Apprenticeship = apps.FirstOrDefault(x => x.Status == PaymentsApprenticeshipStatus.Active);
            var appid = Apprenticeship?.Id;

            await Context.AddEntitiesFromJsonResource<EarningEventModel>($"{prefix}.EarningEvents.json");

            var dlocks = JsonConvert.DeserializeObject<DataLockEventModel[]>(
                Resources.LoadAsString($"{prefix}.Datalocks.json"));
            foreach (var l in dlocks
                .SelectMany(x => x.NonPayablePeriods)
                .SelectMany(x => x.DataLockEventNonPayablePeriodFailures))
            {
                l.ApprenticeshipId = appid;
            }

            await Context.AddEntities(dlocks);

            TimeProvider.Today.Returns(new DateTime(2019, 08, 01));
        }

        protected HttpClient CreateClient() => app.CreateClient();
    }
}