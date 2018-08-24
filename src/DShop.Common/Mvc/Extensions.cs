using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Lockbox.Client.Extensions;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using DShop.Common.Metrics;
using DShop.Common.Options;
using Microsoft.AspNetCore.Http;

namespace DShop.Common.Mvc
{
    public static class Extensions
    {
        public static IMvcCoreBuilder AddCustomMvc(this IServiceCollection services)
        {
            services.AddSingleton<IServiceId, ServiceId>();

            return services
                .AddMvcCore()
                .AddJsonFormatters()
                .AddDataAnnotations()
                .AddApiExplorer()
                .AddDefaultJsonOptions()
                .AddAuthorization();
        }

        public static IMvcCoreBuilder AddDefaultJsonOptions(this IMvcCoreBuilder builder)
            => builder.AddJsonOptions(o =>
                {
                    o.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    o.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                    o.SerializerSettings.DateParseHandling = DateParseHandling.DateTimeOffset;
                    o.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;
                    o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    o.SerializerSettings.Formatting = Formatting.Indented;
                    o.SerializerSettings.Converters.Add(new StringEnumConverter());
                });

        public static IApplicationBuilder UseErrorHandler(this IApplicationBuilder builder)
            => builder.UseMiddleware<ErrorHandlerMiddleware>();

        public static IApplicationBuilder UseServiceId(this IApplicationBuilder builder)
            => builder.Map("/id", c => c.Run(async ctx => 
            {
                using (var scope = c.ApplicationServices.CreateScope())
                {
                    var id = scope.ServiceProvider.GetService<IServiceId>().Id;
                    await ctx.Response.WriteAsync(id);
                }
            }));

        public static IWebHostBuilder UseLockbox(this IWebHostBuilder builder)
            => builder.ConfigureAppConfiguration((ctx, cfg) => 
                {
                    var useLockbox = Environment.GetEnvironmentVariable("USE_LOCKBOX");
                    if (useLockbox?.ToLowerInvariant() == "true")
                    {
                        cfg.AddLockbox();
                    }
                });

        public static T Bind<T>(this T model, Expression<Func<T,object>> expression, object value)
            => model.Bind<T,object>(expression, value);

        public static T BindId<T>(this T model, Expression<Func<T,Guid>> expression)
            => model.Bind<T,Guid>(expression, Guid.NewGuid());

        private static TModel Bind<TModel,TProperty>(this TModel model, Expression<Func<TModel,TProperty>> expression, object value)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
            {
                memberExpression = ((UnaryExpression)expression.Body).Operand as MemberExpression;
            }
            var propertyName = memberExpression.Member.Name.ToLowerInvariant();
            var modelType = model.GetType();
            var field = modelType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                .SingleOrDefault(x => x.Name.ToLowerInvariant().StartsWith($"<{propertyName}>"));
            if (field == null)
            {
                return model;
            }
            field.SetValue(model, value);

            return model;
        }
    }
}