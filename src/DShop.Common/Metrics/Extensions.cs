using System;
using App.Metrics;
using App.Metrics.AspNetCore;
using App.Metrics.AspNetCore.Health;
using DShop.Common.Options;
using Microsoft.AspNetCore.Hosting;

namespace DShop.Common.Metrics
{
    public static class Extensions
    {
        public static IWebHostBuilder UseAppMetrics(this IWebHostBuilder builder)
            => builder.ConfigureMetricsWithDefaults((ctx, b) => 
                {
                    var options = ctx.Configuration.GetOptions<MetricsOptions>("metrics");
                    if (!options.Enabled) {
                        return;
                    }

                    b.Report.ToInfluxDb(o => {
                        o.InfluxDb.Database = options.Database;
                        o.InfluxDb.BaseUri = new Uri(options.Uri);
                        o.InfluxDb.CreateDataBaseIfNotExists = true;
                        o.FlushInterval = TimeSpan.FromSeconds(options.Interval);
                    });
                }
            )
            .UseHealth()
            .UseHealthEndpoints()
            .UseMetricsWebTracking()
            .UseMetricsEndpoints()
            .UseMetrics();
    }
}