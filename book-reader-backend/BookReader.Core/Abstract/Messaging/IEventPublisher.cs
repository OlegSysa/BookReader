namespace BookReader.Core.Abstract.Events
{
    public interface IEventPublisher
    {
        Task PublishAsync<TEvent>(string queueName, TEvent e, string? correlationId, CancellationToken cancellationToken) where TEvent : IBusinessEvent;
    }
}
