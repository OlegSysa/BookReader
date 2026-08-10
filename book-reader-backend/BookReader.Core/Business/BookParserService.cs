using BookReader.Core.Abstract.Repositories;
using BookReader.Core.Abstract.Services;
using BookReader.Core.DTOs.Models;
using BookReader.Core.Entities;
using BookReader.Core.Enums;
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
        public BookParserService(IStorageService storageService,
            IBookRepository repository,
            IConfiguration config,
            ILogger<BookParserService> logger,
            IEnumerable<IParser> parsers) : base(config, logger)
        {
            _storageService = storageService;
            _repository = repository;
            _parsers = parsers;
        }
        public async Task<bool> ParseBook(int bookId, CancellationToken token)
        {
            try
            {
                _logger.LogInformation("[BOOK PROCESSING] STARTED. BookId: {BookId}", bookId);
                var book = await _repository.GetByIdAsync(bookId, token);
                if (book == null)
                {
                    _logger.LogError("Book with id '{BookId}' was not found.", bookId);
                    return false;
                }
                var parser = GetParser(book.OriginalFileName);
                if (parser == null)
                {
                    _logger.LogError("[BOOK PROCESSING] Parser is NULL");
                    return false;
                }

                book.Status = BookStatus.ParseProcessing;
                await _repository.SaveChangesAsync();

                var chapters = await parser.ParseFile(book.StoragePath);
                _logger.LogInformation("[BOOK PROCESSING] PARSED. BookId: {BookId}", bookId);
                var storageRootPath = _config["Storage:ParsedBooksPath"] ?? string.Empty;

                var savingResult = await _storageService.SaveParsedBookToStorageAsync(book.UserId, book.Id,
                    storageRootPath,
                        chapters,
                        token);
                _logger.LogInformation("[BOOK PROCESSING] SAVED PARSED RESULT. BookId: {BookId}, STATUS:{Status}", bookId, savingResult.Status);
                book.ParsedFilesPath = savingResult.Path;
                book.Status = BookStatus.Ready;
                await _repository.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("[BOOK PROCESSING] FAILED. Message: {Message}", e.Message);
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
