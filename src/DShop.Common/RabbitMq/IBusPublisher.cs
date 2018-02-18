using System.Threading.Tasks;
using DShop.Messages.Commands;
using DShop.Messages.Events;

namespace DShop.Common.RabbitMq
{
    public interface IBusPublisher
    {
        Task PublishCommandAsync<TCommand>(TCommand command, ICorrelationContext context = null) where TCommand : ICommand;
        Task PublishEventAsync<TEvent>(TEvent @event) where TEvent : IEvent;
    }
}