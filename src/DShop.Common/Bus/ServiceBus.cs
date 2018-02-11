using RawRabbit;
using System.Threading.Tasks;
using DShop.Common.IoC;
using RawRabbit.Enrichers.MessageContext;
using DShop.Common.Handlers;

namespace DShop.Common.Bus
{
    internal class ServiceBus : IPublishBus, ISubscribeBus
    {
        private readonly IDependencyResolver _dependencyResolver;
        private readonly IBusClient _busClient;
        private readonly ServiceBusOptions _serviceBusOptions;

        public ServiceBus(IDependencyResolver dependencyResolver, IBusClient busClient, ServiceBusOptions serviceBusOptions)
        {
            _dependencyResolver = dependencyResolver;
            _busClient = busClient;
            _serviceBusOptions = serviceBusOptions;
        }

        async Task IPublishBus.PublishCommandAsync<TCommand>(TCommand command, ICorrelationContext context)
            => await _busClient.PublishAsync(command, ctx => ctx.UseMessageContext(context ?? new CorrelationContext()));


        async Task IPublishBus.PublishEventAsync<TEvent>(TEvent @event)
            => await _busClient.PublishAsync(@event);

        void ISubscribeBus.SubscribeToCommand<TCommand>()
            => _busClient.SubscribeAsync<TCommand, CorrelationContext>(async (command, ctx) =>
            {
                var commandHandler = _dependencyResolver.Resolve<ICommandHandler<TCommand>>();
                await commandHandler.HandleAsync(command, ctx);
            },
            ctx => ctx.UseSubscribeConfiguration(cfg => cfg.FromDeclaredQueue(q => q.WithName(_serviceBusOptions.QueueName))));


        void ISubscribeBus.SubscribeToEvent<TEvent>()
            => _busClient.SubscribeAsync<TEvent, CorrelationContext>(async (@event, ctx) =>
            {
                var eventHandler = _dependencyResolver.Resolve<IEventHandler<TEvent>>();
                await eventHandler.HandleAsync(@event, ctx);
            },
            ctx => ctx.UseSubscribeConfiguration(cfg => cfg.FromDeclaredQueue(q => q.WithName(_serviceBusOptions.QueueName))));
    }
}