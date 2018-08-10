using DShop.Common.RabbitMq;
using DShop.Common.Messages;
using System.Threading.Tasks;

namespace DShop.Common.Handlers
{
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        Task HandleAsync(TCommand command, ICorrelationContext context);
    }
}