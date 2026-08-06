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
            var book = await _repository.GetByIdAsync(bookId, token);
            if (book == null)
            {
                _logger.LogError("Book with id '{BookId}' was not found.", bookId);
                return false;
            }
            var parser = GetParser(book.OriginalFileName);
            if (parser == null)
                return false;

            book.Status = BookStatus.ParseProcessing;
            await _repository.SaveChangesAsync();

            var chapters = await parser.ParseFile(book.StoragePath);
            var storageRootPath = _config["Storage:ParsedBooksPath"] ?? string.Empty;
            
            var savingResult = await _storageService.SaveParsedBookToStorageAsync(book.UserId, book.Id,
                storageRootPath,
                    chapters,
                    token);

            book.ParsedFilesPath = savingResult.Path;
            book.Status = BookStatus.Ready;
            await _repository.SaveChangesAsync();

            return true;
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
