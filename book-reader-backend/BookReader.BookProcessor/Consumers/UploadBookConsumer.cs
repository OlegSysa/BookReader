using BookReader.Core.Abstract.Services;
using BookReader.Core.Events;
using MassTransit;

namespace BookReader.BookProcessor.Consumers
{
    public class UploadBookConsumer : IConsumer<BookProcessingEvent>
    {
        private readonly IBookParserService _bookParserService;
        public UploadBookConsumer(IBookParserService bookParserService)
        {
            _bookParserService = bookParserService;
        }
        public async Task Consume(ConsumeContext<BookProcessingEvent> context)
        {

            await _bookParserService.ParseBook(
                context.Message.UserId,
                context.Message.BookId,
                context.CancellationToken);
        }
    }
}
