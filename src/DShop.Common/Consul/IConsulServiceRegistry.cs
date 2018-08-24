using System.Threading.Tasks;
using Consul;

namespace DShop.Common.Consul
{
    public interface IConsulServiceRegistry
    {
        Task<AgentService> GetAsync(string name);
    }
}