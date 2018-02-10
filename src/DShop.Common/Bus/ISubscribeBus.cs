using DShop.Messages.Commands;
using DShop.Messages.Events;

namespace DShop.Common.Bus
{
    public interface ISubscribeBus
    {
        void SubscribeToCommand<TCommand>() where TCommand : class, ICommand;
        void SubscribeToEvent<TEvent>() where TEvent : class, IEvent;
    }
}