using BookReader.Core.Abstract.Services;
using BookReader.Core.Events;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Infrastructure.Services.Messaging.Handlers
{
    public class UploadBookConsumer : IConsumer<BookUploadedEvent>
    {
        private readonly IBookParserService _bookParserService;

        public UploadBookConsumer(IBookParserService bookParserService)
        {
            _bookParserService = bookParserService;
        }
        public async Task Consume(ConsumeContext<BookUploadedEvent> context)
        {
            await _bookParserService.ParseBook(
            context.Message.BookId,
            context.CancellationToken);
        }
    }
}
