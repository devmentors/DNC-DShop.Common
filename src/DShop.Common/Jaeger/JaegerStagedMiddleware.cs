using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using DShop.Common.RabbitMq;
using Jaeger;
using OpenTracing;
using RawRabbit.Pipe;
using RawRabbit.Pipe.Middleware;

namespace DShop.Common.Jaeger
{
    public class JaegerStagedMiddleware : StagedMiddleware
    {
        private readonly ITracer _tracer;

        public JaegerStagedMiddleware(ITracer tracer)
            => _tracer = tracer;

        public override string StageMarker => RawRabbit.Pipe.StageMarker.MessageRecieved;
        
        
        public override Task InvokeAsync(IPipeContext context, CancellationToken token = new CancellationToken())
        {
//            var correlationContext = (ICorrelationContext) context.GetMessageContext();
//            var spanContext = SpanContext.ContextFromString(correlationContext.SpanContext);
//            
//            throw new System.NotImplementedException();
           
            return Next.InvokeAsync(context, token);
        }

        //private 
    }
}