using System.Threading.Tasks;
using DShop.Messages.Commands;
using DShop.Messages.Events;
using RawRabbit;

namespace DShop.Common.Bus.RabbitMq
{
    public class BusPublisher : IBusPublisher
    {
        private readonly IBusClient _busClient;

        public BusPublisher(IBusClient busClient)
        {
            _busClient = busClient;
        }

        public async Task PublishCommandAsync<T>(T command) where T : ICommand
            => await _busClient.PublishAsync(command);

        public async Task PublishEventAsync<T>(T @event) where T : IEvent
            => await _busClient.PublishAsync(@event);
    }
}