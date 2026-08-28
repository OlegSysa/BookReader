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
        public async Task<bool> ParseBook(int userId, int bookId, CancellationToken token)
        {
            try
            {
                _logger.LogInformation("[BOOK PROCESSING] STARTED. BookId: {BookId}", bookId);
                var book = await _repository.GetByIdAsync(bookId, token);
                if (book == null)
                {
                    var message = $"Book with id '{{BookId}}' was not found.";
                    _logger.LogError(message);
                    throw new Exception(message);
                }
                var parser = GetParser(book.OriginalFileName);
                if (parser == null)
                {
                    var message = "[BOOK PROCESSING] Parser is NULL";
                    _logger.LogError(message);
                    throw new Exception(message);
                }

                book.Status = BookStatus.ProcessingStarted;
                await _repository.SaveChangesAsync();
                await _eventPublisher.PublishAsync("book-notifications", new BookNotificationEvent(userId, bookId, book.Status), CorrelationId, token);

                var bookNode = await parser.ParseFile(book.StoragePath);
                book.ChaptersCount = bookNode.Children.Count();
                book.Status = BookStatus.Parsed;
                await _repository.SaveChangesAsync();
                await _eventPublisher.PublishAsync("book-notifications", new BookNotificationEvent(userId, bookId, book.Status), CorrelationId, token);
                _logger.LogInformation("[BOOK PROCESSING] PARSED. BookId: {BookId}", bookId);
                var savingResult = await _storageService.SaveParsedBookToStorageAsync(book.UserId, book.Id,
                        bookNode,
                        token);
                book.ParsedFilesPath = savingResult.Path;
                book.Status = BookStatus.Ready;
                await _repository.SaveChangesAsync();
                await _eventPublisher.PublishAsync("book-notifications", new BookNotificationEvent(userId, bookId, book.Status), CorrelationId, token);
                _logger.LogInformation("[BOOK PROCESSING] SAVED PARSED RESULT. BookId: {BookId}, STATUS:{Status}", bookId, savingResult.Status);

                return true;
            }
            catch (Exception e)
            {
                var errorMessage = $"[BOOK PROCESSING] Failed parse book. Id: {bookId}. Message: {e.Message}";
                _logger.LogError(errorMessage);
                await _eventPublisher.PublishAsync("book-notifications", new BookNotificationEvent(userId, bookId, BookStatus.Failed, errorMessage), CorrelationId, token);
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
