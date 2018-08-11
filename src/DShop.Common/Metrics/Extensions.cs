using System;
using System.IO;
using App.Metrics;
using App.Metrics.AspNetCore;
using App.Metrics.AspNetCore.Health;
using App.Metrics.Extensions.Configuration;
using DShop.Common.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DShop.Common.Metrics
{
    public static class Extensions
    {
        public static IWebHostBuilder UseAppMetrics(this IWebHostBuilder builder)
            => builder.ConfigureMetricsWithDefaults(b => 
                {
                    var configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .AddEnvironmentVariables()
                        .Build();

                    var options = configuration.GetOptions<MetricsOptions>("metrics");
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