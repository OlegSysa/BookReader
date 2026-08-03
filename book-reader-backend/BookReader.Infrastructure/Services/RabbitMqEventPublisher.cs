using BookReader.Core.Abstract.Events;
using MassTransit;

namespace BookReader.Infrastructure.Services
{
    public class RabbitMqEventPublisher : IEventPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public RabbitMqEventPublisher(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task PublishAsync<TEvent>(
            TEvent e,
            CancellationToken cancellationToken)
            where TEvent : IBusinessEvent
        {
            await _publishEndpoint.Publish(e, cancellationToken);
        }
    }
}
