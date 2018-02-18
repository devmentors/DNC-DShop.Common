using System.Threading.Tasks;

namespace DShop.Common.Mongo
{
    public interface IMongoDbSeeder
    {
        Task SeedAsync();
    }
}