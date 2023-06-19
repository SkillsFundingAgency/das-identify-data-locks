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
            using var context = scope.ServiceProvider.GetService<ArchivedPaymentsDataContext>();
            context.Database.EnsureCreated();
        }

        public async Task Reset()
        {
            using var scope = scopeFactory.CreateScope();
            var configuration = scope.ServiceProvider.GetService<IConfiguration>();
            await checkpoint.Reset(configuration.GetConnectionString("ArchivePaymentsSqlConnectionString"));
        }

        public async Task AddEntities<TEntity>(params TEntity[] entities)
            where TEntity : class
        {
            using var scope = scopeFactory.CreateScope();
            using var context = scope.ServiceProvider.GetService<ArchivedPaymentsDataContext>();

            foreach (var entity in entities)
                context.Add(entity);

            await context.SaveChangesAsync();
        }

        public async Task<TEntity[]> AddEntitiesFromJson<TEntity>(string json)
            where TEntity : class
        {
            var entities = JsonConvert.DeserializeObject<TEntity[]>(json);
            await AddEntities(entities);
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