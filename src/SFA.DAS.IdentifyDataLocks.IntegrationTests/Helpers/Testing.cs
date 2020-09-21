using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.IdentifyDataLocks.IntegrationTests;
using SFA.DAS.IdentifyDataLocks.Web.Infrastructure;
using SFA.DAS.Providers.Api.Client;
using System.IO;
using System.Threading.Tasks;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design",
    "RCS1110:Declare type inside namespace.",
    Justification = "NUnit requires SetUpFixture to be outside a namespace")]
[SetUpFixture]
public static class Testing
{
    private static readonly IConfiguration configuration =
        new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .AddEnvironmentVariables().Build();

    private static IServiceScopeFactory scopeFactory;

    public static ScopedContext Context { get; private set; }

    public static ICommitmentsApiClient CommitmentsApi;
    public static IProviderApiClient ProviderApi;
    public static IAccountApiClient AccountsApi;
    public static ITimeProvider TimeProvider;

    [OneTimeSetUp]
    public static void RunBeforeAnyTests()
    {
        var services = CreateServices();

        scopeFactory = services
            .BuildServiceProvider()
            .GetService<IServiceScopeFactory>();

        Context = new ScopedContext(scopeFactory);
    }

    private static ServiceCollection CreateServices()
    {
        var services = new ServiceCollection();
        new TestableStartup(configuration).ConfigureServices(services);
        services.AddLogging();
        services.AddPages();
        services
            .AddSingleton(configuration)
            .ConfigureMockService(_ => CommitmentsApi)
            .ConfigureMockService(_ => ProviderApi)
            .ConfigureMockService(_ => AccountsApi)
            .ConfigureMockService(_ => TimeProvider);
        return services;
    }

    public static T CreatePage<T>() where T : PageModel
    {
        return scopeFactory.CreateScope().ServiceProvider.GetRequiredService<T>();
    }

    internal static async Task Reset()
    {
        await Context.Reset();
        Context = new ScopedContext(scopeFactory);
        CommitmentsApi = Substitute.For<ICommitmentsApiClient>();
        ProviderApi = Substitute.For<IProviderApiClient>();
        AccountsApi = Substitute.For<IAccountApiClient>();
        TimeProvider = Substitute.For<ITimeProvider>();
    }
}