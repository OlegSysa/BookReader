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
        private readonly IChapterRepository _chapterRepository;
        private readonly IEnumerable<IParser> _parsers;
        public BookParserService(IBookRepository repository,
            IChapterRepository chapterRepository,
            IConfiguration config,
            ILogger<BookParserService> logger,
            IEnumerable<IParser> parsers) : base(config, logger)
        {
            _repository = repository;
            _chapterRepository = chapterRepository;
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

            //var jsonInsight = await parser.ParseFile(book.StoragePath);
            //var convertedJson = JsonSerializer.Serialize(jsonInsight);
            //var chapter = new Chapter()
            //    {
            //        Content = convertedJson,
            //        BookId = bookId,
            //        Created = DateTime.UtcNow,
            //        SelectorIndex = 1
            //    };

            //var res = await _chapterRepository.Add(chapter);
            var chapters = await parser.ParseFile(book.StoragePath);
            var chapterEntities = chapters.Select(c =>
            {
                return new Chapter()
                {
                    Content = c.Value,
                    BookId = bookId,
                    Created = DateTime.UtcNow,
                    SelectorIndex = c.Key
                };
            });
            var res = await _chapterRepository.AddBatchAsync(chapterEntities, token);

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
