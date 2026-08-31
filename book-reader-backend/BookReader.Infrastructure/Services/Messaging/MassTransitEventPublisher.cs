using BookReader.Core.Abstract.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BookReader.Infrastructure.Services.Messaging
{
    public class MassTransitEventPublisher : IEventPublisher
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly ILogger<MassTransitEventPublisher> _logger;

        public MassTransitEventPublisher(ISendEndpointProvider sendEndpointProvider, ILogger<MassTransitEventPublisher> logger)
        {
            _sendEndpointProvider = sendEndpointProvider;
            _logger = logger;
        }

        public async Task PublishAsync<TEvent>(string queueName, TEvent e, string? correlationId, CancellationToken cancellationToken) 
            where TEvent : IBusinessEvent
        {
            _logger.LogInformation("Publishing message. CorrelationId: {CorrelationId}", correlationId);

            var endpoint = await _sendEndpointProvider.GetSendEndpoint(
                new Uri($"queue:{queueName}"));

            await endpoint.Send(e, context =>
            {
                if (!string.IsNullOrEmpty(correlationId))
                {
                    context.CorrelationId = Guid.Parse(correlationId);
                }
            }, cancellationToken);
        }
    }
}
