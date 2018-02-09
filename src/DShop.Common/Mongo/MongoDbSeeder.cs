using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace DShop.Common.Mongo
{
    public class MongoDbSeeder : IMongoDbSeeder
    {
        protected readonly IMongoDatabase Database;

        public MongoDbSeeder(IMongoDatabase database)
        {
            Database = database;
        } 

        public async Task SeedAsync()
        {

            await CustomSeedAsync();
        }

        protected virtual async Task CustomSeedAsync()
        {
            var cursor = await Database.ListCollectionsAsync();
            var collections = await cursor.ToListAsync();
            if (collections.Any())
            {
                return;
            }
            await Task.CompletedTask;
        }
    }
}