using BookReader.Core.Abstract.Repositories;
using BookReader.Core.Abstract.Services;
using BookReader.Core.Business;
using BookReader.Core.DTOs.Models;
using BookReader.Core.Entities;
using BookReader.Core.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BookReader.Core.Services
{
    public class BookService : BaseService<BookService>, IBookService
    {
        private readonly IBookRepository bookRepository;
        private readonly IStorageService storageService;
        public BookService(IBookRepository _bookRepository,
            IStorageService _storageService,
            IConfiguration config,
            ILogger<BookService> logger) : base(config, logger)
        {
            bookRepository = _bookRepository;
            storageService = _storageService;
        }
        public async Task<UploadBookResult> UploadAsync(Stream stream,
            string fileName,
            long fileSize,
            int userId,
            CancellationToken token) 
        {
            try
            {
                _logger.LogInformation("Started uploading book file");
                var savingResult = await storageService.SaveBookToStorageAsync(stream, fileName, token);
                if (savingResult.Status == BookStatus.Failed)
                    return new UploadBookResult(null, savingResult.Status);

                var newBook = new Book() {
                    OriginalFileName = fileName,
                    CreatedAtUtc = DateTime.UtcNow,
                    FileSize = fileSize,
                    Status = BookStatus.SavedToStorage,
                    UserId = userId,
                    StoragePath = savingResult.Path
                };

                var metadataResult = await bookRepository.AddNewBook(newBook);
                return new UploadBookResult(newBook, newBook.Status);
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to upload book file. Exception: {e.Message}");
                return new UploadBookResult(null, BookStatus.Failed);
            }
        } 

        public async Task<IReadOnlyCollection<Book>> GetByUserIdAsync(int userId, CancellationToken token) =>
             await bookRepository.GetByUserIdAsync(userId, token);

    }
}
