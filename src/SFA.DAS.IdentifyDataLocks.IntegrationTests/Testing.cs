using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using Respawn;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.IdentifyDataLocks.IntegrationTests;
using SFA.DAS.IdentifyDataLocks.IntegrationTests.Helpers;
using SFA.DAS.IdentifyDataLocks.Web;
using SFA.DAS.IdentifyDataLocks.Web.Infrastructure;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Providers.Api.Client;
using System.IO;
using System.Threading.Tasks;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design",
    "RCS1110:Declare type inside namespace.",
    Justification = "NUnit requires SetUpFixture to be outside a namespace")]
[SetUpFixture]
public static class Testing
{
    private static readonly IConfigurationRoot configuration =
        new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .AddEnvironmentVariables().Build();

    private static IServiceScopeFactory scopeFactory;
    private static Checkpoint checkpoint;

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

        checkpoint = new Checkpoint
        {
            TablesToIgnore = new[] { "__EFMigrationsHistory" }
        };

        EnsureDatabase();
    }

    private static ServiceCollection CreateServices()
    {
        var services = new ServiceCollection();
        new Startup(configuration).ConfigureServices(services);
        services.AddLogging();
        services.AddPages();
        services
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

    public static async Task AddEntities<TEntity>(params TEntity[] entities)
        where TEntity : class
    {
        using var scope = scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetService<PaymentsDataContext>();

        foreach (var entity in entities)
            context.Add(entity);

        await context.SaveChangesAsync();
    }

    public static async Task<TEntity[]> AddEntitiesFromJson<TEntity>(string json)
        where TEntity : class
    {
        var entities = JsonConvert.DeserializeObject<TEntity[]>(json);
        await AddEntities(entities);
        return entities;
    }

    internal static Task<TEntity[]> AddEntitiesFromJsonResource<TEntity>(string name)
        where TEntity : class
    {
        var json = Resources.LoadAsString(name);
        return AddEntitiesFromJson<TEntity>(json);
    }

    private static void EnsureDatabase()
    {
        using var scope = scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetService<PaymentsDataContext>();

        context.Database.EnsureCreated();
    }

    internal static async Task Reset()
    {
        await checkpoint.Reset(configuration.GetConnectionString("PaymentsDatabase"));
        CommitmentsApi = Substitute.For<ICommitmentsApiClient>();
        ProviderApi = Substitute.For<IProviderApiClient>();
        AccountsApi = Substitute.For<IAccountApiClient>();
        TimeProvider = Substitute.For<ITimeProvider>();
    }
}