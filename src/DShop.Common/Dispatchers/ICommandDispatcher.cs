using System.Threading.Tasks;
using DShop.Messages.Commands;

namespace DShop.Common.Dispatchers
{
    public interface ICommandDispatcher
    {
         Task DispatchAsync<T>(T command) where T : ICommand;
    }
}