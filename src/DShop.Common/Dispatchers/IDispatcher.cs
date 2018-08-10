using System.Threading.Tasks;
using DShop.Common.Types;
using DShop.Common.Messages;

namespace DShop.Common.Dispatchers
{
    public interface IDispatcher
    {
        Task SendAsync<TCommand>(TCommand command) where TCommand : ICommand;
        Task<TResult> QueryAsync<TQuery, TResult>(TQuery query) where TQuery : IQuery<TResult>;
    }
}