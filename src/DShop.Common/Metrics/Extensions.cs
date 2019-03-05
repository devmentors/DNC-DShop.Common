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
        private static bool _initialized;

        public static IWebHostBuilder UseAppMetrics(this IWebHostBuilder webHostBuilder)
        {
            if (_initialized)
            {
                return webHostBuilder;
            }

            return webHostBuilder
                .ConfigureMetricsWithDefaults((context, builder) =>
                {
                    var metricsOptions = context.Configuration.GetOptions<MetricsOptions>("metrics");
                    if (!metricsOptions.Enabled)
                    {
                        return;
                    }

                    _initialized = true;
                    builder.Configuration.Configure(cfg =>
                        {
                            var tags = metricsOptions.Tags;
                            if (tags == null)
                            {
                                return;
                            }

                            tags.TryGetValue("app", out var app);
                            tags.TryGetValue("env", out var env);
                            tags.TryGetValue("server", out var server);
                            cfg.AddAppTag(string.IsNullOrWhiteSpace(app) ? null : app);
                            cfg.AddEnvTag(string.IsNullOrWhiteSpace(env) ? null : env);
                            cfg.AddServerTag(string.IsNullOrWhiteSpace(server) ? null : server);
                            foreach (var tag in tags)
                            {
                                if (!cfg.GlobalTags.ContainsKey(tag.Key))
                                {
                                    cfg.GlobalTags.Add(tag.Key, tag.Value);
                                }
                            }
                        }
                    );

                    if (metricsOptions.InfluxEnabled)
                    {
                        builder.Report.ToInfluxDb(o =>
                        {
                            o.InfluxDb.Database = metricsOptions.Database;
                            o.InfluxDb.BaseUri = new Uri(metricsOptions.InfluxUrl);
                            o.InfluxDb.CreateDataBaseIfNotExists = true;
                            o.FlushInterval = TimeSpan.FromSeconds(metricsOptions.Interval);
                        });
                    }
                })
                .UseHealth()
                .UseHealthEndpoints()
                .UseMetricsWebTracking()
                .UseMetrics((context, options) =>
                {
                    var metricsOptions = context.Configuration.GetOptions<MetricsOptions>("metrics");
                    if (!metricsOptions.Enabled)
                    {
                        return;
                    }

                    if (!metricsOptions.PrometheusEnabled)
                    {
                        return;
                    }

                    options.EndpointOptions = endpointOptions =>
                    {
                        switch (metricsOptions.PrometheusFormatter?.ToLowerInvariant() ?? string.Empty)
                        {
                            case "protobuf":
                                endpointOptions.MetricsEndpointOutputFormatter =
                                    new MetricsPrometheusProtobufOutputFormatter();
                                break;
                            default:
                                endpointOptions.MetricsEndpointOutputFormatter =
                                    new MetricsPrometheusTextOutputFormatter();
                                break;
                        }
                    };

                });
        }
    }
}