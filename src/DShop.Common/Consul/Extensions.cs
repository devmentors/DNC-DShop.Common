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
        public static IServiceCollection AddConsul(this IServiceCollection services)
        {
            IConfiguration configuration;
            using (var serviceProvider = services.BuildServiceProvider())
            {
                configuration = serviceProvider.GetService<IConfiguration>();
            }
            services.Configure<ConsulOptions>(configuration.GetSection("consul"));
            services.AddTransient<IConsulServiceRegistry, ConsulServiceRegistry>();
            services.AddTransient<ConsulServiceDiscoveryMessageHandler>();
            services.AddHttpClient<ConsulHttpClient>()
                .AddHttpMessageHandler<ConsulServiceDiscoveryMessageHandler>();

            return services.AddSingleton<IConsulClient>(s => new ConsulClient(cfg =>
            {
                var options = s.GetRequiredService<IOptions<ConsulOptions>>().Value;
                if (!string.IsNullOrEmpty(options.Endpoint))
                {
                    cfg.Address = new Uri(options.Endpoint);
                }
            }));
        }

        //Returns unique service ID used for deregistering service.
        public static string UseConsul(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var applicationLifetime = scope.ServiceProvider.GetService<IApplicationLifetime>();
                var uniqueId = scope.ServiceProvider.GetService<IServiceId>().Id;
                var options = scope.ServiceProvider.GetService<IOptions<ConsulOptions>>();
                var client = scope.ServiceProvider.GetService<IConsulClient>();
                var serviceName = options.Value.Service;
                var serviceId = $"{serviceName}:{uniqueId}";
                var address = options.Value.Address;
                var port = options.Value.Port;
                var pingEndpoint = string.IsNullOrWhiteSpace(options.Value.PingEndpoint) ? "ping" : options.Value.PingEndpoint;
                var pingInterval = options.Value.PingInterval <= 0 ? 5 : options.Value.PingInterval;
                var deregisterInterval = options.Value.DeregisterInterval <= 0 ? 10 : options.Value.DeregisterInterval;
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
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(deregisterInterval),
                    HTTP = $"{address}{(port > 0 ? $":{port}" : string.Empty)}/{pingEndpoint}"
                    };
                    registration.Checks = new[] { check };
                }

                client.Agent.ServiceRegister(registration);

                return serviceId;
            }
        }

        
    }
}