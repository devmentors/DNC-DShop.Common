using Autofac;
using DShop.Common.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DShop.Common.Databases.Mongo
{
    public static class Extensions
    {
        public static void AddMongoDB(this ContainerBuilder builder, MongoDbOptions options)
        {
            builder.RegisterInstance(options).SingleInstance();

            builder.RegisterInstance(new MongoClient(options.ConnectionString)).SingleInstance();

            builder.Register(context =>
            {
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