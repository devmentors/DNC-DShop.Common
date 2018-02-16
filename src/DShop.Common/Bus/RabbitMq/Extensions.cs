using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using DShop.Common.Handlers;
using DShop.Messages.Commands;
using DShop.Messages.Events;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using RabbitMQ.Client.Exceptions;
using RawRabbit;
using RawRabbit.Configuration;
using RawRabbit.Enrichers.Polly;
using RawRabbit.Enrichers.Polly.Services;
using RawRabbit.Instantiation;
using RawRabbit.Pipe;

namespace DShop.Common.Bus.RabbitMq
{
    public static class Extensions
    {
        public static IRabbitMqBuilder UseRabbitMq(this IApplicationBuilder app)
            => new RabbitMqBuilder(app);

        public static void AddRabbitMq(this ContainerBuilder builder)
        {
            builder.Register(context =>
            {
                var configuration = context.Resolve<IConfiguration>();
                var section = configuration.GetSection("rabbitmq");
                var rawRabbitConfiguration = new RawRabbitConfiguration();
                section.Bind(rawRabbitConfiguration);

                return rawRabbitConfiguration;
            }).SingleInstance();

            var assembly = Assembly.GetCallingAssembly();
            builder.RegisterAssemblyTypes(assembly)
                .AsClosedTypesOf(typeof(IEventHandler<>))
                .InstancePerDependency();
            builder.RegisterAssemblyTypes(assembly)
                .AsClosedTypesOf(typeof(ICommandHandler<>))
                .InstancePerDependency();
            builder.RegisterType<BusPublisher>().As<IBusPublisher>()
                .InstancePerDependency();

            ConfigureBus(builder);
        }

        private static void ConfigureBus(ContainerBuilder builder)
        {
            builder.Register<IInstanceFactory>(context =>
                RawRabbitFactory.CreateInstanceFactory(new RawRabbitOptions
                {
                    DependencyInjection = ioc => ioc.AddSingleton(context.Resolve<RawRabbitConfiguration>())
                })).SingleInstance();
            builder.Register(context => context.Resolve<IInstanceFactory>().Create());
        }

        public interface IRabbitMqBuilder
        {
            IRabbitMqBuilder SubscribeCommand<T>(string exchangeName = null) where T : ICommand;
            IRabbitMqBuilder SubscribeEvent<T>(string exchangeName = null) where T : IEvent;
        }

        public class RabbitMqBuilder : IRabbitMqBuilder
        {
            private readonly IBusClient _busClient;
            private readonly IServiceProvider _serviceProvider;

            public RabbitMqBuilder(IApplicationBuilder app)
            {
                _serviceProvider = app.ApplicationServices.GetService<IServiceProvider>();
                _busClient = _serviceProvider.GetService<IBusClient>();
            }

            public IRabbitMqBuilder SubscribeCommand<T>(string exchangeName = null) where T : ICommand
            {
                _busClient.SubscribeAsync<T>(msg => _serviceProvider
                    .GetService<ICommandHandler<T>>().HandleAsync(msg, null),
                        ctx => ctx.UseSubscribeConfiguration(cfg => 
                            cfg.FromDeclaredQueue(q => q.WithName(GetExchangeName<T>(exchangeName)))));

                return this;
            }

            public IRabbitMqBuilder SubscribeEvent<T>(string exchangeName = null) where T : IEvent
            {
                _busClient.SubscribeAsync<T>(msg => _serviceProvider
                    .GetService<IEventHandler<T>>().HandleAsync(msg, null),
                        ctx => ctx.UseSubscribeConfiguration(cfg => 
                            cfg.FromDeclaredQueue(q => q.WithName(GetExchangeName<T>(exchangeName)))));

                return this;
            }
        }

        private static string GetExchangeName<T>(string name = null)
            => string.IsNullOrWhiteSpace(name)
                ? $"{Assembly.GetEntryAssembly().GetName()}/{typeof(T).Name}"
                : $"{name}/{typeof(T).Name}";
    }
}