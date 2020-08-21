using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using NUnit.Framework;
using Respawn;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.LearnerDataMismatches.Web;
using SFA.DAS.LearnerDataMismatches.Web.Pages;
using SFA.DAS.Payments.Application.Repositories;
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

    [OneTimeSetUp]
    public static void RunBeforeAnyTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddScoped<LearnerModel>();
        new Startup(configuration).ConfigureServices(services);

        services.RemoveAll(typeof(ICommitmentsApiClient));
        services.AddScoped(_ => CommitmentsApi);

        scopeFactory = services.BuildServiceProvider().GetService<IServiceScopeFactory>();

        checkpoint = new Checkpoint
        {
            TablesToIgnore = new[] { "__EFMigrationsHistory" }
        };

        EnsureDatabase();
    }

    public static T Create<T>() where T : PageModel
    {
        return scopeFactory.CreateScope().ServiceProvider.GetRequiredService<T>();
    }

    public static async Task AddAsync<TEntity>(TEntity entity)
        where TEntity : class
    {
        using var scope = scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetService<PaymentsDataContext>();

        context.Add(entity);

        await context.SaveChangesAsync();
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
    }
}