using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Respawn;
using SFA.DAS.IdentifyDataLocks.IntegrationTests.Helpers;
using SFA.DAS.Payments.Application.Repositories;
using System.Threading.Tasks;

namespace SFA.DAS.IdentifyDataLocks.IntegrationTests
{
    public class ScopedContext
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly Checkpoint checkpoint;

        public ScopedContext(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
            this.checkpoint = new Checkpoint
            {
                TablesToIgnore = new[] { "__EFMigrationsHistory" }
            };
            EnsureDatabase();
        }

        private void EnsureDatabase()
        {
            using var scope = scopeFactory.CreateScope();
            var archiveContextFactory = scope.ServiceProvider.GetService<ArchiveContextFactory>();
            using var archiveContext = archiveContextFactory.CreateDbContext();
            archiveContext.Database.EnsureCreated();
            var currentContextFactory = scope.ServiceProvider.GetService<CurrentPeriodContextFactory>();
            using var currentContext = currentContextFactory.CreateDbContext();
            currentContext.Database.EnsureCreated();
        }

        public async Task Reset()
        {
            using var scope = scopeFactory.CreateScope();
            var configuration = scope.ServiceProvider.GetService<IConfiguration>();
            await checkpoint.Reset(configuration.GetConnectionString("ArchivePaymentsSqlConnectionString"));
            await checkpoint.Reset(configuration.GetConnectionString("CurrentPaymentsSqlConnectionString"));
        }

        public async Task AddEntities<TEntity>(params TEntity[] entities)
            where TEntity : class
        {
            await AddEntities(false, entities);
        }

        public async Task AddEntities<TEntity>(bool currentPeriod, params TEntity[] entities)
            where TEntity : class
        {
            PaymentsDataContext context;
            using var scope = scopeFactory.CreateScope();
            if (currentPeriod) 
            {
                var factory = scope.ServiceProvider.GetService<CurrentPeriodContextFactory>();
                context = factory.CreateDbContext();
            }
            else
            {
                var factory = scope.ServiceProvider.GetService<ArchiveContextFactory>();
                context = factory.CreateDbContext();
            }

            foreach (var entity in entities)
                context.Add(entity);

            await context.SaveChangesAsync();
        }

        public async Task<TEntity[]> AddEntitiesFromJson<TEntity>(string json)
            where TEntity : class
        {
            var entities = JsonConvert.DeserializeObject<TEntity[]>(json);
            await AddEntities(false, entities);
            return entities;
        }

        internal Task<TEntity[]> AddEntitiesFromJsonResource<TEntity>(string name)
            where TEntity : class
        {
            var json = Resources.LoadAsString(name);
            return AddEntitiesFromJson<TEntity>(json);
        }
    }
}