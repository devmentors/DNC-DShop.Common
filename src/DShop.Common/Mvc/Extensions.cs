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

namespace DShop.Common.Mvc
{
    public static class Extensions
    {
        public static IMvcBuilder AddDefaultJsonOptions(this IMvcBuilder builder)
            => builder.AddJsonOptions(opts =>
                {
                    opts.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    opts.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                    opts.SerializerSettings.DateParseHandling = DateParseHandling.DateTimeOffset;
                    opts.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;
                    opts.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    opts.SerializerSettings.Formatting = Formatting.Indented;
                    opts.SerializerSettings.Converters.Add(new StringEnumConverter());
                });

        public static IApplicationBuilder UseErrorHandler(this IApplicationBuilder builder)
            => builder.UseMiddleware<ErrorHandlerMiddleware>();

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