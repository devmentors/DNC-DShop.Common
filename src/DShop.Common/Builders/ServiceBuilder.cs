using System;
using System.IO;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using DShop.Common.Bus;
using DShop.Common.IoC;
using DShop.Common.Databases.Mongo;
using DShop.Common.Options;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DShop.Common.Builders
{
    public class ServiceBuilder : IServiceBuilder, IIoCServiceBuilder, IDatabaseServiceBuilder, IBusServiceBuilder
    {
        private readonly IWebHostBuilder _webHostBuilder;
        private readonly IConfiguration _configuration;
        private static ContainerBuilder _containerBuilder;
        private static IContainer _container;
        private Action<ISubscribeBus> _subscriptions;

        private ServiceBuilder(IWebHostBuilder webHostBuilder)
        {
            _webHostBuilder = webHostBuilder;
            _configuration = BuildServiceConfiguration();
            _containerBuilder = new ContainerBuilder();
        }

        public static IServiceBuilder Create<TStartup>(string[] args) where TStartup : class
        {
            var webHostBuilder = WebHost
                .CreateDefaultBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<TStartup>();

            return new ServiceBuilder(webHostBuilder);
        }

        public static IServiceProvider GetServiceProvider(IServiceCollection services)
        {
            var containerBuilder = new ContainerBuilder();

            if (services != null)
            {
                containerBuilder.Populate(services);
            }               

            containerBuilder.Update(_container);
            return new AutofacServiceProvider(_container);
        }

        IIoCServiceBuilder IServiceBuilder.WithPort(int port)
        {
            _webHostBuilder.UseUrls($"http://*:{port}");
            return this;
        }

        IDatabaseServiceBuilder IIoCServiceBuilder.WithAutofac(Action<ContainerBuilder> registerDependencies)
        {
            registerDependencies(_containerBuilder);
            _containerBuilder.RegisterType<AutofacDependencyResolver>().As<IDependencyResolver>();
            return this;
        }

        IBusServiceBuilder IDatabaseServiceBuilder.WithMongoDb(string settingsSectionName)
        {
            var options = _configuration.GetOptions<MongoDbOptions>(settingsSectionName);
            _containerBuilder.AddMongoDB();
            return this;
        }

        IBusServiceBuilder IDatabaseServiceBuilder.WithNoDatabase()
            => this;

        IServiceBuilder IBusServiceBuilder.WithServiceBus(string settingsSectionName, Action<ISubscribeBus> subscriptions)
        {
            var settings = _configuration.GetOptions<ServiceBusOptions>(settingsSectionName);
            _containerBuilder.RegisterServiceBus(settings);
            _subscriptions = subscriptions;
            return this;
        }

        IWebHost IServiceBuilder.Build()
        {
            _container = _containerBuilder.Build();
            var webHost = _webHostBuilder.Build();

            var subscribeBus = _container.Resolve<ISubscribeBus>();
            _subscriptions?.Invoke(subscribeBus);

            return webHost;
        }

        private IConfiguration BuildServiceConfiguration()
            => new ConfigurationBuilder()
                    .SetBasePath(Environment.CurrentDirectory)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();
    }
}
