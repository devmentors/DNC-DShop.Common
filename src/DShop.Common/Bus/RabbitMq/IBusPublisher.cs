using System.Threading.Tasks;
using DShop.Messages.Commands;
using DShop.Messages.Events;

namespace DShop.Common.Bus.RabbitMq
{
    public interface IBusPublisher
    {
        Task PublishCommandAsync<T>(T command) where T : ICommand;
        Task PublishEventAsync<T>(T @event) where T : IEvent;
    }
}