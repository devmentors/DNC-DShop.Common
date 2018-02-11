using System.Threading.Tasks;
using DShop.Messages.Commands;
using DShop.Messages.Events;

namespace DShop.Common.Bus
{
    public interface IPublishBus
    {
        Task PublishCommandAsync<TCommand>(TCommand command, ICorrelationContext context) where TCommand : class, ICommand;
        Task PublishEventAsync<TEvent>(TEvent @event) where TEvent : class, IEvent;
    }
}