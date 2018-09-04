using System.Threading.Tasks;

namespace DShop.Common
{
    public interface IInitializer
    {
        Task InitializeAsync();
    }
}