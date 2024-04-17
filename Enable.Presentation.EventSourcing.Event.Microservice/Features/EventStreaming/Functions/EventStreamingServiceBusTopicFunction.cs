using Azure.Messaging.ServiceBus;
using Enable.Presentation.EventSourcing.EventStreaming.Features.EventStreaming.Mediatr.Requests;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Enable.Presentation.EventSourcing.EventStreaming.Features.EventStreaming.Functions;

public class EventStreamingServiceBusTopicFunction(ILogger<EventStreamingServiceBusTopicFunction> logger, IMediator mediator)
{
    private readonly ILogger<EventStreamingServiceBusTopicFunction> _logger = logger;
    private readonly IMediator _mediator = mediator;

    [Function(nameof(EventStreamingServiceBusTopicFunction))]
    public async Task Run
    (
        [ServiceBusTrigger("events", "event-sourcing-subscription", Connection = "EventSourcingConnectionString")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions
    )
    {
        await _mediator.Send(new SendEventsToEventBus());
        await messageActions.CompleteMessageAsync(message);
    }
}
