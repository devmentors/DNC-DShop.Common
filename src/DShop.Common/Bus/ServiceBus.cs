using RawRabbit;
using System.Threading.Tasks;
using DShop.Common.IoC;
using DShop.Messages.Commands;
using DShop.Messages.Events;

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

        async Task IPublishBus.PublishCommandAsync<TCommand>(TCommand command)
            => await _busClient.PublishAsync(command);


        async Task IPublishBus.PublishEventAsync<TEvent>(TEvent @event)
            => await _busClient.PublishAsync(@event);

        void ISubscribeBus.SubscribeToCommand<TCommand>()
            => _busClient.SubscribeAsync<TCommand>(async command =>
            {
                var commandHandler = _dependencyResolver.Resolve<ICommandHandler<TCommand>>();
                await commandHandler.HandleAsync(command);
            },
               ctx => ctx.UseSubscribeConfiguration(cfg => cfg.FromDeclaredQueue(q => q.WithName(_serviceBusOptions.QueueName))));


        void ISubscribeBus.SubscribeToEvent<TEvent>()
            => _busClient.SubscribeAsync<TEvent>(async @event =>
            {
                var eventHandler = _dependencyResolver.Resolve<IEventHandler<TEvent>>();
                await eventHandler.HandleAsync(@event);
            },
               ctx => ctx.UseSubscribeConfiguration(cfg => cfg.FromDeclaredQueue(q => q.WithName(_serviceBusOptions.QueueName))));
    }
}