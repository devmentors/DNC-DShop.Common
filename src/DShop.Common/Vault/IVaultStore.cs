using System.Threading.Tasks;

namespace DShop.Common.Vault
{
    public interface IVaultStore
    {
        Task<T> GetDefaultAsync<T>();
        Task<T> GetAsync<T>(string key);
    }
}