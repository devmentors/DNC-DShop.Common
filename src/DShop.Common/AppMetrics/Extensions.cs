using System;
using App.Metrics.Configuration;
using App.Metrics.Extensions.Reporting.InfluxDB;
using App.Metrics.Extensions.Reporting.InfluxDB.Client;
using App.Metrics.Reporting.Interfaces;
using DShop.Common.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DShop.Common.AppMetrics
{
    public static class Extensions
    {
        public static void AddAppMetrics(this IServiceCollection services)
        {
            IConfiguration configuration;
            using (var serviceProvider = services.BuildServiceProvider())
            {
                configuration = serviceProvider.GetService<IConfiguration>();
            }
            var options = configuration.GetOptions<AppMetricsOptions>("appMetrics");
            services.AddSingleton(options);
            if (!options.Enabled)
            {
                return;
            }

            var database = options.Database;
            var uri = new Uri(options.Uri);
            services.AddMetrics (o => {
                    o.WithGlobalTags((tags, info) =>
                    {
                        tags.Add("app", info.EntryAssemblyName);
                        tags.Add("env", options.Env);
                    });
                })
                .AddHealthChecks()
                .AddReporting(f => {
                    f.AddInfluxDb(new InfluxDBReporterSettings
                    {
                        InfluxDbSettings = new InfluxDBSettings(database, uri),
                        ReportInterval = TimeSpan.FromSeconds(options.Interval)
                    });
                })
                .AddMetricsMiddleware(o => o.IgnoredHttpStatusCodes = new [] { 404 });
        }

        public static void UseAppMetrics(this IApplicationBuilder app, 
            IApplicationLifetime applicationLifetime)
        {
            var enabled = false;
            using (var scope = app.ApplicationServices.CreateScope())
            {
                enabled = scope.ServiceProvider.GetService<AppMetricsOptions>().Enabled;
            }
            if (!enabled)
            {
                return;
            }
            app.UseMetrics();
            app.UseMetricsReporting(applicationLifetime);
        }
    }
}