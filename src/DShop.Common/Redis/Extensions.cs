using DShop.Common.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DShop.Common.Redis
{
    public static class Extensions
    {
        public static IServiceCollection AddRedisCache(this IServiceCollection services)
        {
            IConfiguration configuration;
            using (var serviceProvider = services.BuildServiceProvider())
            {
                configuration = serviceProvider.GetService<IConfiguration>();
            }
            var options = configuration.GetOptions<RedisOptions>("redis");
            services.AddDistributedRedisCache(x => 
            {
                x.Configuration = options.ConnectionString;
            });

            return services;
        }
    }
}