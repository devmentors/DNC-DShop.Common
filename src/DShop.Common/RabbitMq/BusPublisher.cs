using System.Threading.Tasks;
using DShop.Messages.Commands;
using DShop.Messages.Events;
using RawRabbit;
using RawRabbit.Enrichers.MessageContext;

namespace DShop.Common.RabbitMq
{
    internal class BusPublisher : IBusPublisher
    {
        private readonly IBusClient _busClient;

        public BusPublisher(IBusClient busClient)
        {
            _busClient = busClient;
        }

        public async Task PublishCommandAsync<TCommand>(TCommand command, ICorrelationContext context) where TCommand : ICommand
            => await _busClient.PublishAsync(command, ctx => ctx.UseMessageContext(context));

        public async Task PublishEventAsync<TEvent>(TEvent @event) where TEvent : IEvent
            => await _busClient.PublishAsync(@event);
    }
}