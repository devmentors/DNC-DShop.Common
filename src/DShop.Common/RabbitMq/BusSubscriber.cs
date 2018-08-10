using DShop.Common.Handlers;
using DShop.Common.Messages;
using Microsoft.AspNetCore.Builder;
using RawRabbit;
using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace DShop.Common.RabbitMq
{
    internal class BusSubscriber : IBusSubscriber
    {
        private readonly IBusClient _busClient;
        private readonly IServiceProvider _serviceProvider;

        public BusSubscriber(IApplicationBuilder app)
        {
            _serviceProvider = app.ApplicationServices.GetService<IServiceProvider>();
            _busClient = _serviceProvider.GetService<IBusClient>();
        }

        public IBusSubscriber SubscribeCommand<TCommand>(string queueName = null) where TCommand : ICommand
        {
            _busClient.SubscribeAsync<TCommand, CorrelationContext>((command, ctx) =>
            {
                var commandHandler = _serviceProvider.GetService<ICommandHandler<TCommand>>();
                return commandHandler.HandleAsync(command, ctx);

            }, ctx => ctx.UseSubscribeConfiguration(cfg => cfg.FromDeclaredQueue(q => q.WithName(GetQueueName<TCommand>(queueName)))));

            return this;
        }

        public IBusSubscriber SubscribeEvent<TEvent>(string queueName = null) where TEvent : IEvent
        {
            _busClient.SubscribeAsync<TEvent, CorrelationContext>((@event, ctx) =>
            {
                var eventHandler = _serviceProvider.GetService<IEventHandler<TEvent>>();
                return eventHandler.HandleAsync(@event, ctx);

            }, ctx => ctx.UseSubscribeConfiguration(cfg => cfg.FromDeclaredQueue(q => q.WithName(GetQueueName<TEvent>(queueName)))));

            return this;
        }

        private static string GetQueueName<T>(string name = null)
            => (string.IsNullOrWhiteSpace(name)
                ? $"{Assembly.GetEntryAssembly().GetName().Name}/{typeof(T).Name.Underscore()}"
                : $"{name}/{typeof(T).Name.Underscore()}").ToLowerInvariant();
    }
}
