using BookReader.Core.Abstract.Events;
using BookReader.Core.Abstract.Services;
using BookReader.Core.Events;

namespace BookReader.Infrastructure.Services.Messaging.Handlers
{
    public class BookUploadedEventHandler : IEventHandler<BookUploadedEvent>
    {
        private readonly IBookParserService _bookParserService;
        public BookUploadedEventHandler(IBookParserService bookParserService)
        {
            _bookParserService = bookParserService;
        }
        public Task HandleAsync(BookUploadedEvent e, CancellationToken token)
        {
            return _bookParserService.ParseBook(e.BookId, token);
        }
    }
}
