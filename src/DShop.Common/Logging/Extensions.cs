using System;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

namespace DShop.Common.Logging
{
    public static class Extensions
    {
        public static IWebHostBuilder UseLogging(this IWebHostBuilder webHostBuilder, string applicationName = null)
            => webHostBuilder.UseSerilog((context, loggerConfiguration) =>
            {
                var appOptions = context.Configuration.GetOptions<AppOptions>("app");
                var elkOptions = context.Configuration.GetOptions<ElkOptions>("elk");
                var seqOptions = context.Configuration.GetOptions<SeqOptions>("seq");
                var serilogOptions = context.Configuration.GetOptions<SerilogOptions>("serilog");
                if (!Enum.TryParse<LogEventLevel>(serilogOptions.Level, true, out var level))
                {
                    level = LogEventLevel.Information;
                }

                applicationName = string.IsNullOrWhiteSpace(applicationName) ? appOptions.Name : applicationName;
                loggerConfiguration.Enrich.FromLogContext()
                    .MinimumLevel.Is(level)
                    .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                    .Enrich.WithProperty("ApplicationName", applicationName);
                Configure(loggerConfiguration, level, elkOptions, seqOptions, serilogOptions);
            });

        private static void Configure(LoggerConfiguration loggerConfiguration, LogEventLevel level,
            ElkOptions elkOptions, SeqOptions seqOptions, SerilogOptions serilogOptions)
        {
            if (elkOptions.Enabled)
            {
                loggerConfiguration.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elkOptions.Url))
                {
                    MinimumLogEventLevel = level,
                    AutoRegisterTemplate = true,
                    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
                    IndexFormat = string.IsNullOrWhiteSpace(elkOptions.IndexFormat)
                        ? "logstash-{0:yyyy.MM.dd}"
                        : elkOptions.IndexFormat,
                    ModifyConnectionSettings = connectionConfiguration =>
                        elkOptions.BasicAuthEnabled
                            ? connectionConfiguration.BasicAuthentication(elkOptions.Username, elkOptions.Password)
                            : connectionConfiguration
                });
            }

            if (seqOptions.Enabled)
            {
                loggerConfiguration.WriteTo.Seq(seqOptions.Url, apiKey: seqOptions.ApiKey);
            }

            if (serilogOptions.ConsoleEnabled)
            {
                loggerConfiguration.WriteTo.Console();
            }
        }
    }
}