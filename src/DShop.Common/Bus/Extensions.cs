using System;
using System.Collections.Generic;
using Autofac;
using RawRabbit.Configuration;
using RawRabbit.DependencyInjection.Autofac;
using RawRabbit.Enrichers.MessageContext;
using RawRabbit.Instantiation;
using RawRabbit.Serialization;

namespace DShop.Common.Bus
{
    internal static class Extensions
    {        
        internal static void RegisterServiceBus(this ContainerBuilder containerBuilder, ServiceBusOptions options)
        {
            containerBuilder.RegisterInstance(options).SingleInstance();
            containerBuilder.RegisterType<ServiceBus>().As<IPublishBus>();
            containerBuilder.RegisterType<ServiceBus>().As<ISubscribeBus>();

            containerBuilder.RegisterRawRabbit(new RawRabbitOptions
            {
                ClientConfiguration = new RawRabbitConfiguration
                {
                    Username = options.Username,
                    Password = options.Password,
                    Port = options.Port,
                    VirtualHost = "/",
                    Hostnames = new List<string> { "localhost" },
                    Queue = new GeneralQueueConfiguration()
                    {
                        Durable = true,
                    },
                    RequestTimeout = new TimeSpan(0, 10, 0)
                },
                DependencyInjection = ioc => ioc.AddSingleton<ISerializer, ServiceBusSerializer>(),
                Plugins = p => p
                    .UseMessageContext<CorrelationContext>()
                    .UseContextForwarding()
            });
        }
    }
}