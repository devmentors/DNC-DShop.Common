using System;
using System.Threading.Tasks;
using Autofac;
using DShop.Common.Handlers;
using DShop.Common.Types;

namespace DShop.Common.Dispatchers
{
    public class QueryDispatcher : IQueryDispatcher
    {
        private readonly IComponentContext _context;

        public QueryDispatcher(IComponentContext context)
        {
            _context = context;
        }

        public async Task<TResult> DispatchAsync<TQuery, TResult>(TQuery query) where TQuery : IQuery<TResult>
            => await _context.Resolve<IQueryHandler<TQuery, TResult>>().HandleAsync(query);
    }
}