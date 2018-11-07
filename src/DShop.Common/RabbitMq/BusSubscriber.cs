using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DShop.Common.Handlers;
using DShop.Common.Messages;
using DShop.Common.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RawRabbit;
using RawRabbit.Common;
using RawRabbit.Enrichers.MessageContext.Subscribe;
using RawRabbit.Pipe;
using RawRabbit.Pipe.Middleware;

namespace DShop.Common.RabbitMq {
    public class BusSubscriber : IBusSubscriber
    {
        private readonly ILogger _logger;
        private readonly IBusClient _busClient;
        private readonly IServiceProvider _serviceProvider;
        private readonly string _defaultNamespace;
        private readonly int _retries;
        private readonly int _retryInterval;

        public BusSubscriber(IApplicationBuilder app)
        {
            _logger = app.ApplicationServices.GetService<ILogger<BusSubscriber>>();
            _serviceProvider = app.ApplicationServices.GetService<IServiceProvider>();
            _busClient = _serviceProvider.GetService<IBusClient>();
            var options = _serviceProvider.GetService<RabbitMqOptions>();
            _defaultNamespace = options.Namespace;
            _retries = options.Retries >= 0 ? options.Retries : 3;
            _retryInterval = options.RetryInterval > 0 ? options.RetryInterval : 2;
        }

        public IBusSubscriber SubscribeCommand<TCommand>(string @namespace = null, string queueName = null,
            Func<TCommand, DShopException, IRejectedEvent> onFailure = null)
            where TCommand : ICommand
        {
            _busClient.SubscribeAsync<TCommand, CorrelationContext>(async (command, correlationContext) =>
                {
                    var commandName = command.GetType().Name;
                    var retryMessage = correlationContext.Retries == 0
                        ? string.Empty
                        : $"Retry: {correlationContext.Retries}'.";
                    _logger.LogInformation(
                        $"Handling command: '{commandName}' with correlation id: '{correlationContext.Id}'. {retryMessage}");
                    var commandHandler = _serviceProvider.GetService<ICommandHandler<TCommand>>();
                    try
                    {
                        await commandHandler.HandleAsync(command, correlationContext);
                        _logger.LogInformation(
                            $"Handled command: '{commandName}' with correlation id: '{correlationContext.Id}'. {retryMessage}");

                        return new Ack();
                    }
                    catch (Exception exception)
                    {
                        // if (exception is DShopException dShopException && onFailure != null)
                        // {
                        //     var rejectedEvent = onFailure(command, dShopException);
                        //     await _busClient.PublishAsync(rejectedEvent);
                        //     _logger.LogInformation($"Published rejected event: '{rejectedEvent.GetType().Name}' " +
                        //                            $"for command: '{commandName}' with correlation id: '{correlationContext.Id}'.");

                        //     return new Ack();
                        // }

                        if (_retries == 0)
                        {
                            throw;
                        }

                        if (correlationContext.Retries >= _retries)
                        {
                            throw new Exception(
                                $"Unable to handle command: '{commandName}' with correlation id: '{correlationContext.Id}' " +
                                $"after {correlationContext.Retries} retries.", exception);
                        }

                        _logger.LogInformation(
                            $"Unable to handle command: '{commandName}' with correlation id: '{correlationContext.Id}', " +
                            $"retry {correlationContext.Retries}/{_retries}...");

                        return Retry.In(TimeSpan.FromSeconds(_retryInterval));
                    }
                },
                ctx => ctx.UseSubscribeConfiguration(cfg =>
                            cfg.FromDeclaredQueue(q => q.WithName(GetQueueName<TCommand>(@namespace, queueName)))));

            return this;
        }

        public IBusSubscriber SubscribeEvent<TEvent>(string @namespace = null, string queueName = null,
            Func<TEvent, DShopException, IRejectedEvent> onFailure = null)
            where TEvent : IEvent
        {
            _busClient.SubscribeAsync<TEvent, CorrelationContext>((@event, correlationContext) =>
                {
                    var eventHandler = _serviceProvider.GetService<IEventHandler<TEvent>>();

                    return eventHandler.HandleAsync(@event, correlationContext);
                },
                ctx => ctx.UseSubscribeConfiguration(cfg =>
                    cfg.FromDeclaredQueue(q => q.WithName(GetQueueName<TEvent>(@namespace, queueName)))));

            return this;
        }

        private string GetQueueName<T>(string @namespace = null, string name = null)
        {
            @namespace = string.IsNullOrWhiteSpace(@namespace)
                ? (string.IsNullOrWhiteSpace(_defaultNamespace) ? string.Empty : _defaultNamespace)
                : @namespace;

            var separatedNamespace = string.IsNullOrWhiteSpace(@namespace) ? string.Empty : $"{@namespace}.";

            return (string.IsNullOrWhiteSpace(name)
                ? $"{Assembly.GetEntryAssembly().GetName().Name}/{separatedNamespace}{typeof(T).Name.Underscore()}"
                : $"{name}/{separatedNamespace}{typeof(T).Name.Underscore()}").ToLowerInvariant();
        }
    }
}