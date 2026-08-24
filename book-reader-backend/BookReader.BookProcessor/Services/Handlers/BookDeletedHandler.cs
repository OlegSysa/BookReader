using BookReader.BookProcessor.Abstract;
using BookReader.Core.Abstract.Repositories;
using BookReader.Core.Abstract.Services;
using BookReader.Core.DTOs.Models;
using BookReader.Core.Entities;
using BookReader.Core.Enums;
using BookReader.Core.Extensions;
using System.Text.Json;

namespace BookReader.BookProcessor.Services.Handlers
{
    public class BookDeletedHandler : IOutboxMessageHandler
    {
        private readonly IStorageService _storageService;
        private readonly ILogger<BookDeletedHandler> _logger;
        private readonly IOutboxMessageRepository _outboxMessageRepository;
        public BookDeletedHandler(IStorageService storageService,
            IOutboxMessageRepository outboxMessageRepository,
            ILogger<BookDeletedHandler> logger)
        {
            _logger = logger;
            _storageService = storageService;
            _outboxMessageRepository = outboxMessageRepository;
        }

        public OutboxMessageType Type => OutboxMessageType.BookDeleted;

        public async Task HandleAsync(IEnumerable<OutboxMessage> messages, CancellationToken token)
        {
            if (messages.IsNullOrEmpty())
                return;

            foreach (var message in messages)
            {
                try
                {
                    var data = JsonSerializer.Deserialize<BookDeletedPayload>(message.Payload);
                    if (data == null)
                    {
                        message.LastError = "Invalid BookDeleted payload";
                        message.ProcessedAtUtc = DateTime.UtcNow;
                        continue;
                    }

                    var deletedBook = await _storageService.DeleteBookFromStorage(data.UserId, data.OriginalFileName);
                    var deletedParsedBook = await _storageService.DeleteParsedBookFromStorage(data.UserId, data.BookId, token);

                    if (deletedBook && deletedParsedBook)
                        message.ProcessedAtUtc = DateTime.UtcNow;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to delete book from storage");
                    message.RetryCount++;
                    message.LastError = e.Message;

                }
                finally
                {
                    await _outboxMessageRepository.SaveChangesAsync();
                }
            }
        }
    }
}
