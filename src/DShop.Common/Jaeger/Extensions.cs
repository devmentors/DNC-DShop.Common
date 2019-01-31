using System;
using Jaeger;
using Jaeger.Reporters;
using Jaeger.Samplers;
using Jaeger.Senders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTracing;
using OpenTracing.Util;

namespace DShop.Common.Jaeger
{
public static class Extensions
    {
        public static IServiceCollection AddJaegerTracer(this IServiceCollection services)
        {
            var options = GetJaegerOptions(services);

            if (!options.Enabled)
            {
                return services;
            }
            
            services.AddSingleton(sp =>
            {
                var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

                var reporter = new RemoteReporter
                    .Builder()
                    .WithSender(new UdpSender(options.UdpHost, options.UdpPort, options.MaxPacketSize))
                    .WithLoggerFactory(loggerFactory)
                    .Build();

                var sampler = GetSampler(options);
                
                var tracer = new Tracer
                    .Builder(options.ServiceName)
                    .WithReporter(reporter)
                    .WithSampler(sampler)
                    .Build();
                
                GlobalTracer.Register(tracer);
                return tracer;
            });

            return services;
        }

        private static JaegerOptions GetJaegerOptions(IServiceCollection services)
        {
            using (var serviceProvider = services.BuildServiceProvider())
            {
                var configuration = serviceProvider.GetService<IConfiguration>();
                services.Configure<JaegerOptions>(configuration.GetSection("jaeger"));
                return configuration.GetOptions<JaegerOptions>("jaeger");
            }
        }

        private static ISampler GetSampler(JaegerOptions options)
        {
            switch (options.Sampler)
            {
                case "const": return new ConstSampler(true);
                case "rate": return new RateLimitingSampler(options.MaxTracesPerSecond);
                case "probabilistic": return new ProbabilisticSampler(options.SamplingRate);
                default: return new ConstSampler(true);
            }
        }

        private static ITracer ConfigureTracer(Action<Tracer.Builder> optionBuilder)
        {
            var appName = System.Reflection.Assembly.GetEntryAssembly().FullName;
            var tracerBuilder = new Tracer.Builder(appName);
            optionBuilder(tracerBuilder);
            var tracer = tracerBuilder.Build();
            GlobalTracer.Register(tracer);
            return tracer;
        }
    }
}