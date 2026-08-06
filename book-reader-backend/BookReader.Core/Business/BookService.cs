using BookReader.Core.Abstract.Events;
using BookReader.Core.Abstract.Repositories;
using BookReader.Core.Abstract.Services;
using BookReader.Core.Business;
using BookReader.Core.DTOs.Models;
using BookReader.Core.Entities;
using BookReader.Core.Enums;
using BookReader.Core.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BookReader.Core.Services
{
    public class BookService : BaseService<BookService>, IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IStorageService _storageService;
        private readonly IEventPublisher _eventPublisher;
        public BookService(IBookRepository bookRepository,
            IStorageService storageService,
            IEventPublisher eventPublisher,
            IConfiguration config,
            ILogger<BookService> logger) : base(config, logger)
        {
            _bookRepository = bookRepository;
            _storageService = storageService;
            _eventPublisher = eventPublisher;
        }
        public async Task<UploadBookResult> UploadAsync(Stream stream,
            UploadBookDetails details,
            CancellationToken token) 
        {
            try
            {
                var isValidFile = await ValidateUploadBookModel(details, token);
                if (!isValidFile)
                {
                    _logger.LogInformation($"Can not upload file. Invalid file details {details.FileName}");
                    return new UploadBookResult(null, BookStatus.Failed);
                }
                var storageRootPath = _config["Storage:BooksPath"] ?? string.Empty;
                var savingResult = await _storageService.SaveBookToStorageAsync(storageRootPath,
                    stream,
                    details.FileName,
                    details.UserId, token);
                if (savingResult.Status == BookStatus.Failed)
                {
                    _logger.LogError($"Can not upload file {details.FileName}. Issue with file storage");
                    return new UploadBookResult(null, savingResult.Status);
                }
                    
                var newBook = new Book() {
                    OriginalFileName = details.FileName,
                    CreatedAtUtc = DateTime.UtcNow,
                    FileSize = details.FileSize,
                    Status = BookStatus.SavedToStorage,
                    UserId = details.UserId,
                    StoragePath = savingResult.Path
                };
                var metadataSavingResult = await _bookRepository.AddNewBook(newBook);
                if (!metadataSavingResult)
                {
                    _logger.LogError($"File was uploaded, but metadata wasn't saved to db. Filename: {details.FileName}");
                    await _storageService.DeleteBookFromStorage(details.UserId, details.FileName);
                    return new UploadBookResult(null, BookStatus.Failed);
                }
                await _eventPublisher.PublishAsync(new BookUploadedEvent(newBook.Id), token);

                return new UploadBookResult(newBook, newBook.Status);
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to upload book file. Exception: {e.Message}");
                return new UploadBookResult(null, BookStatus.Failed);
            }
        } 

        public async Task<IReadOnlyCollection<Book>> GetByUserIdAsync(int userId, CancellationToken token) =>
             await _bookRepository.GetByUserIdAsync(userId, token);
        public async Task<Book?> GetBookByUserAndFileNameAsync(int userId, string fileName, CancellationToken token) =>
             await _bookRepository.GetByUserAndFileNameAsync(userId, fileName, token);

        private async Task<bool> ValidateUploadBookModel(UploadBookDetails details, CancellationToken token)
        {
            if (details.UserId == 0)
                return false;

            var existingBook = await GetBookByUserAndFileNameAsync(details.UserId, details.FileName, token);
            if (existingBook is not null)
                return false;
            var availableExtensions = _config.GetSection("BookExtensions").Get<List<string>>() ?? new List<string>();
            var fileExtension = Path.GetExtension(details.FileName);
            if (!availableExtensions.Contains(fileExtension))
                return false;
            var maxFileSize = _config.GetValue<long>("MaxFileSize");
            if (details.FileSize > maxFileSize)
                return false;

            return true;
        }
    }
}
