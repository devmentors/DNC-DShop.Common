using System;
using Consul;
using DShop.Common.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DShop.Common.Consul
{
    public static class Extensions
    {
        private static readonly string SectionName = "consul";

        public static IServiceCollection AddConsul(this IServiceCollection services)
        {
            IConfiguration configuration;
            using (var serviceProvider = services.BuildServiceProvider())
            {
                configuration = serviceProvider.GetService<IConfiguration>();
            }

            var options = configuration.GetOptions<ConsulOptions>(SectionName);
            services.Configure<ConsulOptions>(configuration.GetSection(SectionName));
            services.AddTransient<IConsulServicesRegistry, ConsulServicesRegistry>();
            services.AddTransient<ConsulServiceDiscoveryMessageHandler>();
            services.AddHttpClient<ConsulHttpClient>()
                .AddHttpMessageHandler<ConsulServiceDiscoveryMessageHandler>();

            return services.AddSingleton<IConsulClient>(c => new ConsulClient(cfg =>
            {
                if (!string.IsNullOrEmpty(options.Url))
                {
                    cfg.Address = new Uri(options.Url);
                }
            }));
        }

        //Returns unique service ID used for removing the service from registry.
        public static string UseConsul(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var options = scope.ServiceProvider.GetService<IOptions<ConsulOptions>>();
                if (!options.Value.Enabled)
                {
                    return string.Empty;
                }

                var uniqueId = scope.ServiceProvider.GetService<IServiceId>().Id;
                var client = scope.ServiceProvider.GetService<IConsulClient>();
                var serviceName = options.Value.Service;
                var serviceId = $"{serviceName}:{uniqueId}";
                var address = options.Value.Address;
                var port = options.Value.Port;
                var pingEndpoint = string.IsNullOrWhiteSpace(options.Value.PingEndpoint)
                    ? "ping"
                    : options.Value.PingEndpoint;
                var pingInterval = options.Value.PingInterval <= 0 ? 5 : options.Value.PingInterval;
                var removeAfterInterval =
                    options.Value.RemoveAfterInterval <= 0 ? 10 : options.Value.RemoveAfterInterval;
                var registration = new AgentServiceRegistration
                {
                    Name = serviceName,
                    ID = serviceId,
                    Address = address,
                    Port = port,
                };
                if (options.Value.PingEnabled)
                {
                    var check = new AgentServiceCheck
                    {
                        Interval = TimeSpan.FromSeconds(pingInterval),
                        DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(removeAfterInterval),
                        HTTP = $"{address}{(port > 0 ? $":{port}" : string.Empty)}/{pingEndpoint}"
                    };
                    registration.Checks = new[] {check};
                }

                client.Agent.ServiceRegister(registration);

                return serviceId;
            }
        }
    }
}