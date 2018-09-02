using System;
using App.Metrics;
using App.Metrics.AspNetCore;
using App.Metrics.AspNetCore.Health;
using App.Metrics.Formatters.Prometheus;
using Microsoft.AspNetCore.Hosting;

namespace DShop.Common.Metrics
{
    public static class Extensions
    {
        public static IWebHostBuilder UseAppMetrics(this IWebHostBuilder webHostBuilder)
            => webHostBuilder.ConfigureMetricsWithDefaults((context, builder) =>
                    {
                        var options = context.Configuration.GetOptions<MetricsOptions>("metrics");
                        if (!options.Enabled)
                        {
                            return;
                        }

                        builder.Report.ToInfluxDb(o =>
                        {
                            o.InfluxDb.Database = options.Database;
                            o.InfluxDb.BaseUri = new Uri(options.InfluxUrl);
                            o.InfluxDb.CreateDataBaseIfNotExists = true;
                            o.FlushInterval = TimeSpan.FromSeconds(options.Interval);
                        });
                        if (options.PrometheusEnabled)
                        {

                            builder
                                .OutputMetrics.AsPrometheusPlainText()
                                .OutputMetrics.AsPrometheusProtobuf();
                        }
                    }
                )
                .UseHealth()
                .UseHealthEndpoints()
                .UseMetricsWebTracking()
                .UseMetricsEndpoints((context, setup) =>
                {
                    var options = context.Configuration.GetOptions<MetricsOptions>("metrics");
                    if (options.PrometheusEnabled)
                    {
                        setup.MetricsTextEndpointOutputFormatter = new MetricsPrometheusTextOutputFormatter();
                        setup.MetricsEndpointOutputFormatter = new MetricsPrometheusProtobufOutputFormatter();
                    }
                })
                .UseMetrics();
    }
}