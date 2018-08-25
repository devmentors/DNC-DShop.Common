using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Consul;

namespace DShop.Common.Consul
{
    public class ConsulServiceDiscoveryMessageHandler : DelegatingHandler
    {
        private readonly IConsulServicesRegistry _servicesRegistry;
        private readonly string _serviceName;
        private readonly bool? _overrideRequestUri;

        public ConsulServiceDiscoveryMessageHandler(IConsulServicesRegistry servicesRegistry,
            string serviceName = null, bool? overrideRequestUri = null)
        {
            _servicesRegistry = servicesRegistry;
            _serviceName = serviceName;
            _overrideRequestUri = overrideRequestUri;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var currentUri = GetUri(request);
            var name = string.IsNullOrWhiteSpace(_serviceName) ? currentUri.Host : _serviceName;
            var service = await _servicesRegistry.GetAsync(name);
            if (service == null)
            {
                throw new ConsulServiceNotFoundException($"Consul service: '{name}' was not found.", name);
            }

            var uriBuilder = new UriBuilder(currentUri)
            {
                Host = service.Address,
                Port = service.Port
            };
            request.RequestUri = uriBuilder.Uri;

            return await base.SendAsync(request, cancellationToken);
        }

        private Uri GetUri(HttpRequestMessage request)
            => string.IsNullOrWhiteSpace(_serviceName)
                ? request.RequestUri
                : _overrideRequestUri == true
                    ? new Uri(
                        $"{request.RequestUri.Scheme}://{_serviceName}/{request.RequestUri.Host}{request.RequestUri.PathAndQuery}")
                    : request.RequestUri;
    }
}