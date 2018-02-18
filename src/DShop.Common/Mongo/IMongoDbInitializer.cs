using System.Threading.Tasks;

namespace DShop.Common.Mongo
{
    public interface IMongoDbInitializer
    {
        Task InitializeAsync();
    }
}