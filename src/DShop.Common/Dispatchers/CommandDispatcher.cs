using System;
using System.Threading.Tasks;
using Autofac;
using DShop.Common.Dispatchers;
using DShop.Common.Handlers;
using DShop.Common.RabbitMq;
using DShop.Messages.Commands;

namespace TDShop.Common.Dispatchers
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IComponentContext _context;

        public CommandDispatcher(IComponentContext context)
        {
            _context = context;
        }

        public async Task DispatchAsync<T>(T command) where T : ICommand
            => await _context.Resolve<ICommandHandler<T>>().HandleAsync(command, CorrelationContext.Empty);
    }
}