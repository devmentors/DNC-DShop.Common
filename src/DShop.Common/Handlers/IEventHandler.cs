using DShop.Common.RabbitMq;
using DShop.Common.Messages;
using System.Threading.Tasks;

namespace DShop.Common.Handlers
{
    public interface IEventHandler<in TEvent> where TEvent : IEvent
    {
        Task HandleAsync(TEvent @event, ICorrelationContext context);
    }
}