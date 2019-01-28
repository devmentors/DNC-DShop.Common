using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Polly;

namespace DShop.Common.Fabio
{
    public class FabioMessageHandler : DelegatingHandler
    {
        private readonly IOptions<FabioOptions> _options;
        private readonly string _servicePath;

        public FabioMessageHandler(IOptions<FabioOptions> options, string serviceName = null)
        {
            if (string.IsNullOrWhiteSpace(options.Value.Url))
            {
                throw new InvalidOperationException("Fabio URL was not provided.");
            }

            _options = options;
            _servicePath = string.IsNullOrWhiteSpace(serviceName) ? string.Empty : $"{serviceName}/";
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            request.RequestUri = GetRequestUri(request);

            return await Policy.Handle<Exception>()
                .WaitAndRetryAsync(RequestRetries, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
                .ExecuteAsync(async () => await base.SendAsync(request, cancellationToken));
        }

        private Uri GetRequestUri(HttpRequestMessage request)
            =>  new Uri($"{_options.Value.Url}/{_servicePath}{request.RequestUri.Host}{request.RequestUri.PathAndQuery}");
        
        private int RequestRetries => _options.Value.RequestRetries <= 0 ? 3 : _options.Value.RequestRetries;
    }
}