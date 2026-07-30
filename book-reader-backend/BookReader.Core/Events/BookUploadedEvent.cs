using BookReader.Core.Abstract.Events;

namespace BookReader.Core.Events
{
    public sealed record BookUploadedEvent(int BookId) : IBusinessEvent;
}
