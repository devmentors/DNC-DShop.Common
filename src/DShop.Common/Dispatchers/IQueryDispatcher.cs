using System.Threading.Tasks;
using DShop.Common.Types;

namespace DShop.Common.Dispatchers
{
    public interface IQueryDispatcher
    {
        Task<TResult> QueryAsync<TResult>(IQuery<TResult> query);
    }
}