using BookReader.Core.Abstract.Events;
using BookReader.Core.Abstract.Repositories;
using BookReader.Core.Abstract.Services;
using BookReader.Core.DTOs.Models;
using BookReader.Core.Entities;
using BookReader.Core.Enums;
using BookReader.Core.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BookReader.Core.Business
{
    public class BookParserService : BaseService<BookParserService>, IBookParserService
    {
        private readonly IBookRepository _repository;
        private readonly IEnumerable<IParser> _parsers;
        private readonly IStorageService _storageService;
        private readonly IEventPublisher _eventPublisher;
        public BookParserService(IStorageService storageService,
            IBookRepository repository,
            IConfiguration config,
            ILogger<BookParserService> logger,
            IEnumerable<IParser> parsers,
            IEventPublisher eventPublisher,
            IHttpContextAccessor httpContextAccessor) : base(config, logger, httpContextAccessor)
        {
            _storageService = storageService;
            _repository = repository;
            _parsers = parsers;
            _eventPublisher = eventPublisher;
        }
        public async Task<bool> ParseBook(int userId, int bookId, string? correlationId, CancellationToken token)
        {
            try
            {
                _logger.LogInformation("[BOOKPROCESSOR] PROCESSING STARTED. BookId: {BookId}", bookId);
                var book = await _repository.GetByIdAsync(bookId, token);
                if (book == null)
                {
                    _logger.LogError(
                        "Book with id {BookId} was not found.",
                        bookId);

                    throw new KeyNotFoundException(
                        $"Book with id '{bookId}' was not found.");
                }
                var parser = GetParser(book.OriginalFileName);
                if (parser == null)
                {
                    _logger.LogError(
                        "[BOOKPROCESSOR] Parser was not found for BookId {BookId}",
                        bookId);

                    throw new InvalidOperationException(
                        $"[BOOKPROCESSOR] Parser was not found for BookId {bookId}");
                }

                book.Status = BookStatus.ProcessingStarted;
                await _repository.SaveChangesAsync();
                await _eventPublisher.PublishAsync("book-notifications", new BookNotificationEvent(userId, bookId, book.Status), correlationId, token);

                var bookNode = await parser.ParseFile(book.StoragePath);
                book.ChaptersCount = bookNode.Children.Count();
                book.Status = BookStatus.Parsed;
                await _repository.SaveChangesAsync();
                await _eventPublisher.PublishAsync("book-notifications", new BookNotificationEvent(userId, bookId, book.Status), correlationId, token);
                _logger.LogInformation("[BOOKPROCESSOR] BOOK PARSED. BookId: {BookId}", bookId);
                var savingResult = await _storageService.SaveParsedBookToStorageAsync(book.UserId, book.Id,
                        bookNode,
                        token);
                book.ParsedFilesPath = savingResult.Path;
                book.Status = BookStatus.Ready;
                await _repository.SaveChangesAsync();
                await _eventPublisher.PublishAsync("book-notifications", new BookNotificationEvent(userId, bookId, book.Status), correlationId, token);
                _logger.LogInformation("[BOOKPROCESSOR] SAVED PARSED RESULT. BookId: {BookId}, STATUS:{Status}", bookId, savingResult.Status);

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(
                    e,
                    "[BOOKPROCESSOR] Failed to parse book. BookId: {BookId}",
                    bookId);

                var errorMessage =
                    $"Failed to parse book with ID {bookId}. Error: {e.Message}";

                await _eventPublisher.PublishAsync(
                    "book-notifications",
                    new BookNotificationEvent(
                        userId,
                        bookId,
                        BookStatus.Failed,
                        errorMessage),
                    correlationId,
                    token);

                return false;
            }
        }

        private IParser? GetParser(string fileName)
        {
            var extension = Path.GetExtension(fileName).TrimStart('.');
            if (string.IsNullOrEmpty(extension) ||
                !_parsers.ToDictionary(p => p.Extension).TryGetValue(Enum.Parse<BookExtension>(extension), out IParser? parser))
            {
                _logger.LogError("Cannot find relevant parser for given file extension: {extension}", extension);
                return null;
            }
            return parser;
        }
    }
}
