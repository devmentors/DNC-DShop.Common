using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DShop.Common.Vault
{
    public static class Extensions
    {
        public static IServiceCollection AddVault(this IServiceCollection services)
        {
            IConfiguration configuration;
            using (var serviceProvider = services.BuildServiceProvider())
            {
                configuration = serviceProvider.GetService<IConfiguration>();
            }
            services.Configure<VaultOptions>(configuration.GetSection("vault"));
            services.AddTransient<IVaultStore, VaultStore>();

            return services;
        }
    }
}