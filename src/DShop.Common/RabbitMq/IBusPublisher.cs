using System.Threading.Tasks;
using DShop.Messages.Commands;
using DShop.Messages.Events;

namespace DShop.Common.RabbitMq
{
    public interface IBusPublisher
    {
        Task PublishCommandAsync<TCommand>(TCommand command, ICorrelationContext context) 
            where TCommand : ICommand;
        Task PublishEventAsync<TEvent>(TEvent @event, ICorrelationContext context) 
            where TEvent : IEvent;
    }
}