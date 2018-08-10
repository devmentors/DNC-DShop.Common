using System.Threading.Tasks;
using DShop.Common.Types;

namespace DShop.Common.Dispatchers
{
    public interface IQueryDispatcher
    {
        Task<TResult> QueryAsync<TQuery,TResult>(TQuery query) where TQuery : IQuery<TResult>;
    }
}