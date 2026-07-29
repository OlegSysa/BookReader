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
        private readonly IBookRepository _bookRepository;
        private readonly IStorageService _storageService;
        public BookService(IBookRepository bookRepository,
            IStorageService storageService,
            IConfiguration config,
            ILogger<BookService> logger) : base(config, logger)
        {
            _bookRepository = bookRepository;
            _storageService = storageService;
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
                
                var savingResult = await _storageService.SaveBookToStorageAsync(stream, details.FileName, details.UserId, token);
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
        public async Task<IReadOnlyCollection<Book>> GetBookByUserAndFileNameAsync(int userId, string fileName, CancellationToken token) =>
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
            var maxFileSize = _config.GetValue<long>("BookSettings:MaxFileSize");
            if (details.FileSize > maxFileSize)
                return false;

            return true;
        }
    }
}
