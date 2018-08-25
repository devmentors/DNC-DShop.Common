using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consul;

namespace DShop.Common.Consul
{
    public class ConsulServicesRegistry : IConsulServicesRegistry
    {
        private readonly IConsulClient _client;
        private readonly IDictionary<string, ISet<string>> _usedServices = new Dictionary<string, ISet<string>>();
        private IDictionary<string, AgentService> _allServices = new Dictionary<string, AgentService>();

        public ConsulServicesRegistry(IConsulClient client)
        {
            _client = client;
        }

        public async Task<AgentService> GetAsync(string name)
        {
            if (_allServices == null || !_allServices.Any())
            {
                await _client.Agent.Services()
                    .ContinueWith(t => _allServices = t.Result.Response);
            }

            var services = GetServices(name);
            if (!services.Any())
            {
                return null;
            }

            if (!_usedServices.ContainsKey(name))
            {
                _usedServices[name] = new HashSet<string>();
            }
            else if (services.Count == _usedServices[name].Count)
            {
                _usedServices[name].Clear();
            }

            var service = services.FirstOrDefault(s => !_usedServices[name].Contains(s.ID));
            if (service != null)
            {
                _usedServices[name].Add(service.ID);
            }

            return service;
        }

        private IList<AgentService> GetServices(string name)
            => _allServices?.Where(s => s.Value.Service.Equals(name,
                       StringComparison.InvariantCultureIgnoreCase))
                   .Select(x => x.Value).ToList() ?? new List<AgentService>();

        private AgentService GetService(string name)
            => _allServices?.FirstOrDefault(s => s.Value.Service.Equals(name,
                StringComparison.InvariantCultureIgnoreCase)).Value;
    }
}