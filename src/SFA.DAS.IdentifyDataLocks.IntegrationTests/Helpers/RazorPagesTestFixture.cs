using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.IdentifyDataLocks.Domain.Services;

namespace SFA.DAS.IdentifyDataLocks.IntegrationTests.Helpers;

public class RazorPagesTestFixture
{
    private static readonly IConfiguration Configuration =
        new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .AddEnvironmentVariables().Build();

    private static IServiceScopeFactory _scopeFactory;

    public static ScopedContext Context { get; private set; }
    public static ICommitmentsApiClient CommitmentsApi;
    public static IProviderService ProviderApi;
    public static IAccountApiClient AccountsApi;
    public static ITimeProvider TimeProvider;

    public static void Reset()
    {
        var services = CreateServices();

        _scopeFactory = services
            .BuildServiceProvider()
            .GetService<IServiceScopeFactory>();

        Context = new ScopedContext(_scopeFactory);
    }

    private static ServiceCollection CreateServices()
    {
        var environment = Substitute.For<IWebHostEnvironment>();
        CommitmentsApi = Substitute.For<ICommitmentsApiClient>();
        ProviderApi = Substitute.For<IProviderService>();
        AccountsApi = Substitute.For<IAccountApiClient>();
        TimeProvider = Substitute.For<ITimeProvider>();
        AccountsApi.GetAccount(Arg.Any<long>()).Returns(new AccountDetailViewModel { DasAccountName = "test", PublicHashedAccountId = "test" });

        var services = new ServiceCollection();
        new TestableStartup(Configuration, environment).ConfigureServices(services);
        services.AddLogging();
        services.AddPages();
        services
            .AddSingleton(Configuration)
            .ConfigureMockService(_ => CommitmentsApi)
            .ConfigureMockService(_ => ProviderApi)
            .ConfigureMockService(_ => AccountsApi)
            .ConfigureMockService(_ => TimeProvider)
            .ConfigurePaymentsAuditDataContext()
            .ConfigurePaymentsAuditDataContext();
        return services;
    }

    public static T CreatePage<T>() where T : PageModel
    {
        return _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<T>();
    }
}