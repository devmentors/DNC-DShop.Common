using System.Reflection;
using Autofac;
using DShop.Common.Handlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using RawRabbit;
using RawRabbit.Common;
using RawRabbit.Enrichers.MessageContext;
using RawRabbit.Instantiation;

namespace DShop.Common.RabbitMq
{
    public static class Extensions
    {
        public static IBusSubscriber UseRabbitMq(this IApplicationBuilder app)
            => new BusSubscriber(app);

        public static void AddRabbitMq(this ContainerBuilder builder)
        {
            builder.Register(context =>
            {
                var configuration = context.Resolve<IConfiguration>();
                var rawRabbitConfiguration = configuration.GetOptions<RabbitMqOptions>("rabbitMq");
                
                return rawRabbitConfiguration;

            }).SingleInstance();

            var assembly = Assembly.GetCallingAssembly();
            builder.RegisterAssemblyTypes(assembly)
                .AsClosedTypesOf(typeof(IEventHandler<>))
                .InstancePerDependency();
            builder.RegisterAssemblyTypes(assembly)
                .AsClosedTypesOf(typeof(ICommandHandler<>))
                .InstancePerDependency();
            builder.RegisterType<Handler>().As<IHandler>()
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
                    DependencyInjection = ioc => 
                    {
                        ioc.AddSingleton<INamingConventions, CustomNamingConventions>();
                        ioc.AddSingleton(context.Resolve<RabbitMqOptions>());
                    },
                    Plugins = p => p
                        .UseAttributeRouting()
                        .UseMessageContext<CorrelationContext>()
                        .UseContextForwarding()

                })).SingleInstance();

            builder.Register(context => context.Resolve<IInstanceFactory>().Create());
        }

        private class CustomNamingConventions : NamingConventions
        {
            public CustomNamingConventions()
            {
                ExchangeNamingConvention = type => type?.Name?.Underscore().ToLowerInvariant() ?? string.Empty;
            }
        }      
    }
}