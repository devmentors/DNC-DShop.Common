using System;
using System.Linq;
using System.Threading.Tasks;
using Consul;

namespace DShop.Common.Consul
{
    public class ConsulServiceRegistry : IConsulServiceRegistry
    {
        private readonly IConsulClient _client;

        public ConsulServiceRegistry(IConsulClient client)
        {
            _client = client;
        }
        
        public async Task<AgentService> GetAsync(string name)
        {
            var services = await _client.Agent.Services();
            if (services.Response == null || !services.Response.Any())
            {
                return null;
            }

            return services.Response.FirstOrDefault(s => s.Value.Service.Equals(name, StringComparison.InvariantCultureIgnoreCase)).Value;
        }
    }
}