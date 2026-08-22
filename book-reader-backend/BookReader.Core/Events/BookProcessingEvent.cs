using BookReader.Core.Abstract.Events;
using BookReader.Core.Enums;

namespace BookReader.Core.Events
{
    public sealed record BookProcessingEvent(int UserId, int BookId) : IBusinessEvent;
}
