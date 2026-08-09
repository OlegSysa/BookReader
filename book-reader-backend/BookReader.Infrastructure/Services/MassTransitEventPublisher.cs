using BookReader.Core.Abstract.Events;
using MassTransit;

namespace BookReader.Infrastructure.Services
{
    public class MassTransitEventPublisher : IEventPublisher
    {
        //private readonly IPublishEndpoint _publishEndpoint;
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public MassTransitEventPublisher(ISendEndpointProvider sendEndpointProvider)
        {
            _sendEndpointProvider = sendEndpointProvider;
        }

        public async Task PublishAsync<TEvent>(TEvent e, CancellationToken cancellationToken) 
            where TEvent : IBusinessEvent
        {
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(
                new Uri("queue:book-processing"));

            await endpoint.Send(e, cancellationToken);
        }
    }
}
