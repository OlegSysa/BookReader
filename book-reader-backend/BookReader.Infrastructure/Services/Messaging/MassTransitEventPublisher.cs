using BookReader.Core.Abstract.Events;
using MassTransit;

namespace BookReader.Infrastructure.Services.Messaging
{
    public class MassTransitEventPublisher : IEventPublisher
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public MassTransitEventPublisher(ISendEndpointProvider sendEndpointProvider)
        {
            _sendEndpointProvider = sendEndpointProvider;
        }

        public async Task PublishAsync<TEvent>(string queueName, TEvent e, string? correlationId, CancellationToken cancellationToken) 
            where TEvent : IBusinessEvent
        {
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
