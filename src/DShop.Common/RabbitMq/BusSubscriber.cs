using DShop.Common.Handlers;
using DShop.Messages.Commands;
using DShop.Messages.Events;
using Microsoft.AspNetCore.Builder;
using RawRabbit;
using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;


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

        public IBusSubscriber SubscribeCommand<TCommand>(string exchangeName = null) where TCommand : ICommand
        {
            _busClient.SubscribeAsync<TCommand, CorrelationContext>((command, ctx) =>
            {
                var commandHandler = _serviceProvider.GetService<ICommandHandler<TCommand>>();
                return commandHandler.HandleAsync(command, ctx);

            }, ctx => ctx.UseSubscribeConfiguration(cfg => cfg.FromDeclaredQueue(q => q.WithName(GetExchangeName<TCommand>(exchangeName)))));

            return this;
        }

        public IBusSubscriber SubscribeEvent<TEvent>(string exchangeName = null) where TEvent : IEvent
        {
            _busClient.SubscribeAsync<TEvent, CorrelationContext>((@event, ctx) =>
            {
                var eventHandler = _serviceProvider.GetService<IEventHandler<TEvent>>();
                return eventHandler.HandleAsync(@event, ctx);

            }, ctx => ctx.UseSubscribeConfiguration(cfg => cfg.FromDeclaredQueue(q => q.WithName(GetExchangeName<TEvent>(exchangeName)))));

            return this;
        }

        private static string GetExchangeName<T>(string name = null)
            => string.IsNullOrWhiteSpace(name)
                ? $"{Assembly.GetEntryAssembly().GetName()}/{typeof(T).Name}"
                : $"{name}/{typeof(T).Name}";
    }
}
