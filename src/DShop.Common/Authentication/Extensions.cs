using System;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace DShop.Common.Authentication
{
    public static class Extensions
    {
        public static void AddJwt(this IServiceCollection services)
        {
            IConfiguration configuration;
            using (var serviceProvider = services.BuildServiceProvider())
            {
                configuration = serviceProvider.GetService<IConfiguration>();
            }
            var section = configuration.GetSection("jwt");
            var options = new JwtOptions();
            section.Bind(options);
            services.AddSingleton(options);
            services.AddSingleton<IJwtHandler, JwtHandler>();
            services.AddAuthentication()
                .AddJwtBearer(cfg =>
                {
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecretKey)),
                        ValidIssuer = options.Issuer,
                        ValidAudience = options.ValidAudience,
                        ValidateAudience = options.ValidateAudience,
                        ValidateLifetime = options.ValidateLifetime
                    };
                });
        }

        public static long ToTimestamp(this DateTime dateTime)
        {
            var centuryBegin = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var expectedDate = dateTime.Subtract(new TimeSpan(centuryBegin.Ticks));

            return expectedDate.Ticks / 10000;
        }
    }
}