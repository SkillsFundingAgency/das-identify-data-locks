using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SFA.DAS.IdentifyDataLocks.Data.Repositories;

namespace SFA.DAS.IdentifyDataLocks.IntegrationTests.Helpers
{
    public class ScopedContext
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public ScopedContext(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        private async Task AddEntities<TEntity>(params TEntity[] entities) where TEntity : class
        {
            using var scope = _scopeFactory.CreateScope();
            {
                var context = scope.ServiceProvider.GetService<PaymentsDataContext>();

                await context.AddRangeAsync(entities);

                await context.SaveChangesAsync();
            }
        }

        internal async Task<TEntity[]> AddEntitiesFromJsonResource<TEntity>(string name) where TEntity : class
        {
            var json = Resources.LoadAsString(name);

            var entities = JsonConvert.DeserializeObject<TEntity[]>(json);
            
            await AddEntities(entities);
            
            return entities;
        }
    }
}