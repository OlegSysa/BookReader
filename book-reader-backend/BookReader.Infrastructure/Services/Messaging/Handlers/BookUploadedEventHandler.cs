using BookReader.Core.Abstract.Events;
using BookReader.Core.Abstract.Services;
using BookReader.Core.Events;

namespace BookReader.Infrastructure.Services.Messaging.Handlers
{
    [Obsolete]
    public class BookUploadedEventHandler : IEventHandler<BookProcessingEvent>
    {
        private readonly IBookParserService _bookParserService;
        public BookUploadedEventHandler(IBookParserService bookParserService)
        {
            _bookParserService = bookParserService;
        }
        public Task HandleAsync(BookProcessingEvent e, CancellationToken token)
        {
            return _bookParserService.ParseBook(e.UserId, e.BookId, token);
        }
    }
}
