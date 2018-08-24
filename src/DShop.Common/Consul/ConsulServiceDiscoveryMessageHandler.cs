using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DShop.Common.Consul
{
    public class ConsulServiceDiscoveryMessageHandler : DelegatingHandler
    {
        private readonly IConsulServiceRegistry _serviceRegistry;

        public ConsulServiceDiscoveryMessageHandler(IConsulServiceRegistry serviceRegistry)
        {
            _serviceRegistry = serviceRegistry;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var currentUri = request.RequestUri;
            var name = currentUri.Host;
            var service = await _serviceRegistry.GetAsync(name);
            if (service == null)
            {
                throw new ConsulServiceNotFoundException($"Service: '{name}' was not found.", name);
            }

            var uriBuilder = new UriBuilder(currentUri)
            {
                Host = service.Address,
                Port = service.Port
            };
            request.RequestUri = uriBuilder.Uri;

            return await base.SendAsync(request, cancellationToken);
        }
    }
}