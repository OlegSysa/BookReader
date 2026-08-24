using BookReader.Core.Entities;
using BookReader.Core.Enums;

namespace BookReader.BookProcessor.Abstract
{
    public interface IOutboxMessageHandler
    {
        OutboxMessageType Type { get; }
        Task HandleAsync(IEnumerable<OutboxMessage> messages, CancellationToken token);
    }
}
