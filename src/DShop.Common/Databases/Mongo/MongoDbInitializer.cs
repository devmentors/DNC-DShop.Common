using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace DShop.Common.Databases.Mongo
{
    public class MongoDbInitializer : IMongoDbInitializer
    {
        private static bool _initialized;
        private readonly bool _seed;
        private readonly IMongoDatabase _database;
        private readonly IMongoDbSeeder _seeder;

        public MongoDbInitializer(IMongoDatabase database, 
            IMongoDbSeeder seeder,
            MongoDbOptions options)
        {
            _database = database;
            _seeder = seeder;
            _seed = options.Seed;
        }

        public async Task InitializeAsync()
        {
            if (_initialized)
            {
                return;
            }
            RegisterConventions();
            _initialized = true;
            if (!_seed)
            {
                return;
            }
            await _seeder.SeedAsync();
        }

        private void RegisterConventions()
        {
            ConventionRegistry.Register("Conventions", new MongoDbConventions(), x => true);
        }

        private class MongoDbConventions : IConventionPack
        {
            public IEnumerable<IConvention> Conventions => new List<IConvention>
            {
                new IgnoreExtraElementsConvention(true),
                new EnumRepresentationConvention(BsonType.String),
                new CamelCaseElementNameConvention()
            };
        }
    }
}