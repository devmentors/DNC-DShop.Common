using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using DShop.Common.Messages;
using DShop.Common.RabbitMq;
using Jaeger;
using OpenTracing;
using OpenTracing.Tag;
using RawRabbit.Pipe;
using RawRabbit.Pipe.Middleware;

namespace DShop.Common.Jaeger
{
    public class JaegerStagedMiddleware : StagedMiddleware
    {
        private readonly ITracer _tracer;

        public JaegerStagedMiddleware(ITracer tracer)
            => _tracer = tracer;

        public override string StageMarker => RawRabbit.Pipe.StageMarker.MessageDeserialized;
        
        
        public override Task InvokeAsync(IPipeContext context, CancellationToken token = new CancellationToken())
        {
            var correlationContext = (ICorrelationContext) context.GetMessageContext();
            var spanContext = SpanContext.ContextFromString(correlationContext.SpanContext);
            var messageType = context.GetMessageType();

            using (var scope = BuildScope(spanContext, messageType))
            {
                var span = scope.Span;
                span.Log($"Processing {messageType.Name}");

                try
                {
                    return Next.InvokeAsync(context, token);
                }
                catch(Exception ex)
                {
                    span.SetTag(Tags.Error, true);
                    span.Log(ex.Message);
                }
            }

            return Next.Next.InvokeAsync(context, token);
        }

        private IScope BuildScope(ISpanContext spanContext, Type messageType)
            => _tracer
                .BuildSpan($"processing-{messageType.Name}")
                .WithTag("message-type", $@"{(messageType.IsAssignableTo<ICommand>()? "command" : "event" )}")
                .AddReference(References.FollowsFrom, spanContext)
                .StartActive(true);
    }
}