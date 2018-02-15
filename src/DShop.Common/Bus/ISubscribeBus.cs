using DShop.Messages.Commands;
using DShop.Messages.Events;

namespace DShop.Common.Bus
{
    public interface ISubscribeBus
    {
        ISubscribeBus SubscribeToCommand<TCommand>() where TCommand : class, ICommand;
        ISubscribeBus SubscribeToEvent<TEvent>() where TEvent : class, IEvent;
    }
}