using DShop.Common.RabbitMq;
using DShop.Messages.Commands;
using System.Threading.Tasks;

namespace DShop.Common.Handlers
{
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        Task HandleAsync(TCommand command, ICorrelationContext context);
    }
}