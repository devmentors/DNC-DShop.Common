using System.Threading.Tasks;
using DShop.Common.Types;
using DShop.Messages.Commands;

namespace DShop.Common.Dispatchers
{
    public class Dispatcher : IDispatcher
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;

        public Dispatcher(ICommandDispatcher commandDispatcher,
            IQueryDispatcher queryDispatcher)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
        }

        public async Task DispatchAsync<TCommand>(TCommand command) where TCommand : ICommand
            => await _commandDispatcher.DispatchAsync(command);

        public async Task<TResult> DispatchAsync<TQuery, TResult>(TQuery query) where TQuery : IQuery<TResult>
            => await _queryDispatcher.DispatchAsync<TQuery, TResult>(query);
    }
}