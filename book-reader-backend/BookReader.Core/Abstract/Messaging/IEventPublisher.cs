namespace BookReader.Core.Abstract.Events
{
    public interface IEventPublisher
    {
        Task PublishAsync<TEvent>(TEvent e, CancellationToken cancellationToken) where TEvent : IBusinessEvent;
    }
}
