using Autofac;
using DShop.Common.Options;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace DShop.Common.Mongo
{
    public static class Extensions
    {
        public static void AddMongoDB(this ContainerBuilder builder)
        {
            builder.Register(context =>
            {
                var configuration = context.Resolve<IConfiguration>();
                var options = configuration.GetOptions<MongoDbOptions>("mongo");
                return options;

            }).SingleInstance();

            builder.Register(context =>
            {
                var options = context.Resolve<MongoDbOptions>();
                return new MongoClient(options.ConnectionString);

            }).SingleInstance();

            builder.Register(context =>
            {
                var options = context.Resolve<MongoDbOptions>();
                var client = context.Resolve<MongoClient>();
                return client.GetDatabase(options.Database);

            }).InstancePerLifetimeScope();

            builder.RegisterType<MongoDbInitializer>()
                .As<IMongoDbInitializer>()
                .InstancePerLifetimeScope();

            builder.RegisterType<MongoDbSeeder>()
                .As<IMongoDbSeeder>()
                .InstancePerLifetimeScope();
        }
    }
}