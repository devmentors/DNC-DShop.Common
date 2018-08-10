using System.Threading.Tasks;
using DShop.Common.Messages;

namespace DShop.Common.Dispatchers
{
    public interface ICommandDispatcher
    {
         Task SendAsync<T>(T command) where T : ICommand;
    }
}