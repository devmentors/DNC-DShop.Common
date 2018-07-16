using System.Threading.Tasks;
using DShop.Common.Types;
using DShop.Messages.Commands;

namespace DShop.Common.Dispatchers
{
    public interface IDispatcher
    {
        Task DispatchAsync<TCommand>(TCommand command) where TCommand : ICommand;
        Task<TResult> DispatchAsync<TQuery, TResult>(TQuery query) where TQuery : IQuery<TResult>;
    }
}