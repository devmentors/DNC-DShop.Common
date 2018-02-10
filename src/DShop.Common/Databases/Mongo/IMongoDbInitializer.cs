using System.Threading.Tasks;

namespace DShop.Common.Databases.Mongo
{
    public interface IMongoDbInitializer
    {
        Task InitializeAsync();
    }
}