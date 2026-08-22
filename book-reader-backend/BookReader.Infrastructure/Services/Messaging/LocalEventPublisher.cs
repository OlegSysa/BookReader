using BookReader.Core.Abstract.Events;
using Microsoft.Extensions.DependencyInjection;

namespace BookReader.Infrastructure.Services.Messaging
{
    [Obsolete]
    public class LocalEventPublisher : IEventPublisher
    {
        private readonly IServiceScopeFactory _factory;
        public LocalEventPublisher(IServiceScopeFactory factory)
        {
            _factory = factory;
        }
        public async Task PublishAsync<TEvent>(string queueName, TEvent e, CancellationToken cancellationToken) where TEvent : IBusinessEvent
        {
            using var scope = _factory.CreateScope();
            var handlers = scope.ServiceProvider.GetServices<IEventHandler<TEvent>>();
            foreach (var handler in handlers)
            {
                await handler.HandleAsync(e, cancellationToken);
            }
        }
    }
}
