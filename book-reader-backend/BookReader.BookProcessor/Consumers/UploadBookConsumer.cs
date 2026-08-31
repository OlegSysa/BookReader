using BookReader.Core.Abstract.Services;
using BookReader.Core.Events;
using MassTransit;
using Serilog.Context;

namespace BookReader.BookProcessor.Consumers
{
    public class UploadBookConsumer : IConsumer<BookProcessingEvent>
    {
        private readonly ILogger<UploadBookConsumer> _logger;

        public UploadBookConsumer(ILogger<UploadBookConsumer> logger)
        {
            _logger = logger;
        }
        private readonly IBookParserService _bookParserService;
        public UploadBookConsumer(IBookParserService bookParserService)
        {
            _bookParserService = bookParserService;
        }
        public async Task Consume(ConsumeContext<BookProcessingEvent> context)
        {
            _logger.LogInformation("Received message. MessageId: {MessageId}, CorrelationId: {CorrelationId}",
                context.MessageId,
                context.CorrelationId);
            using (Serilog.Context.LogContext.PushProperty("CorrelationId", context.CorrelationId))
            {
                await _bookParserService.ParseBook(
                    context.Message.UserId,
                    context.Message.BookId,
                    context.CancellationToken);
            }
        }
    }
}
