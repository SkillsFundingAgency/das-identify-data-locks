using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Respawn;
using SFA.DAS.LearnerDataMismatches.Web;
using SFA.DAS.LearnerDataMismatches.Web.Pages;
using SFA.DAS.Payments.Application.Repositories;
using System.IO;

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

    [OneTimeSetUp]
    public static void RunBeforeAnyTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddScoped<LearnerModel>();
        new Startup(configuration).ConfigureServices(services);

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

    private static void EnsureDatabase()
    {
        using var scope = scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetService<PaymentsDataContext>();

        context.Database.EnsureCreated();
    }
}