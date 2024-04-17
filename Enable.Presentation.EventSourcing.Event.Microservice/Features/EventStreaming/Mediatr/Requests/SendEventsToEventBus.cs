using Confluent.Kafka;
using Enable.Presentation.EventSourcing.Infrastructure.Layer.Data.Context;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using System.Text.Json;

namespace Enable.Presentation.EventSourcing.EventStreaming.Features.EventStreaming.Mediatr.Requests
{

    /// <summary>
    /// A mediatr request to send events to the event bus for event streaming to be consumed by other microservices
    /// </summary>
    public class SendEventsToEventBus : IRequest<Unit>
    {
    }

    /// <summary>
    /// A mediatr request to send events to the event bus for event streaming to be consumed by other microservices
    /// </summary>
    public class SendEventsToEventBusHandler(IEnablePresentationDbContext enablePresentationDbContext, ILogger<SendEventsToEventBusHandler> logger, IConfiguration configuration) : IRequestHandler<SendEventsToEventBus, Unit>
    {
        private readonly IEnablePresentationDbContext _enablePresentationDbContext = enablePresentationDbContext;
        private readonly ILogger<SendEventsToEventBusHandler> _logger = logger;
        private readonly string _topic = "events";
        private readonly int _eventStreamingLimit = configuration.GetValue<int>("Kafka:eventStreamingLimit");
        private readonly ProducerConfig _producerConfig = new()
        {
            BootstrapServers = configuration.GetConnectionString("Kafka:bootstrapServers"),
            SaslPassword = configuration.GetValue<string>("Kafka:username"),
            SaslUsername = configuration.GetValue<string>("Kafka:password"),
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslMechanism = SaslMechanism.Plain,
            EnableIdempotence = true,
            LingerMs = 100,
            MessageSendMaxRetries = 3,
            RetryBackoffMs = 100,
            TransactionTimeoutMs = 30000,
            BatchNumMessages = 100000,
            TransactionalId = Guid.NewGuid().ToString()
        };

        public async Task<Unit> Handle(SendEventsToEventBus request, CancellationToken cancellationToken)
        {
            var hasEvents = true;
            using var producer = new ProducerBuilder<string, string>(_producerConfig).Build();

            while (hasEvents)
            {
                var outboxEvents = _enablePresentationDbContext
                    .EventOutBox
                    .OrderBy(e => e.SequenceNumber)
                    .Take(_eventStreamingLimit)
                    .ToImmutableList();

                if (!outboxEvents.Any())
                {
                    hasEvents = false;
                    break;
                }

                try
                {
                    var eventsToProduce = outboxEvents.Select(e =>
                    {
                        return producer.ProduceAsync(_topic, new Message<string, string>
                        {
                            Key = e.Name,
                            Value = JsonSerializer.Serialize(e.Payload)
                        }, cancellationToken);
                    });

                    producer.BeginTransaction();

                    await Task.WhenAll(eventsToProduce);

                    producer.CommitTransaction();

                    _enablePresentationDbContext.EventOutBox.RemoveRange(outboxEvents);
                    await _enablePresentationDbContext.SaveChangesAsync(cancellationToken);
                }
                catch (Exception exception)
                {
                    producer.AbortTransaction();
                    _logger.LogError(exception, "Error sending events to event bus");
                }

            }

            _logger.LogInformation("Events sent to event bus");
            return Unit.Value;
        }
    }
}
