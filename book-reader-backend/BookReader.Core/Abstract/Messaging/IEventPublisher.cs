namespace BookReader.Core.Abstract.Events
{
    public interface IEventPublisher
    {
        Task PublishAsync<TEvent>(string queueName, TEvent e, CancellationToken cancellationToken) where TEvent : IBusinessEvent;
    }
}
