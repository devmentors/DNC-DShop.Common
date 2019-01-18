using System.Threading.Tasks;
using Consul;

namespace DShop.Common.Consul
{
    public interface IConsulServicesRegistry
    {
        Task<AgentService> GetAsync(string name);
    }
}