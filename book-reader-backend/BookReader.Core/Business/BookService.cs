using BookReader.Core.Abstract.Events;
using BookReader.Core.Abstract.Repositories;
using BookReader.Core.Abstract.Services;
using BookReader.Core.Business;
using BookReader.Core.DTOs.Models;
using BookReader.Core.Entities;
using BookReader.Core.Enums;
using BookReader.Core.Events;
using BookReader.Core.Extensions.Mappings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.NetworkInformation;
using System.Net.WebSockets;

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
        public async Task<ServiceResult<UploadBookResult>> UploadAsync(Stream stream,
            UploadBookDetails details,
            CancellationToken token)
        {
            try
            {
                var isValidFile = await ValidateUploadBookModel(details, token);
                if (!isValidFile)
                {
                    var message = $"Can not upload file. Invalid file details {details.FileName}";
                    _logger.LogError(message);
                    return new ServiceResult<UploadBookResult>(
                        new UploadBookResult(null, BookStatus.Failed), 
                        message);
                }
                var storageRootPath = _config["Storage:BooksPath"] ?? string.Empty;
                var savingResult = await _storageService.SaveBookToStorageAsync(storageRootPath,
                    stream,
                    details.FileName,
                    details.UserId, token);
                if (savingResult.Status == BookStatus.Failed)
                {
                    var message = $"Can not upload file {details.FileName}. Issue with file storage";
                    _logger.LogError(message);
                    return new ServiceResult<UploadBookResult>(
                        new UploadBookResult(null, BookStatus.Failed),
                        message);
                }

                var newBook = new Book()
                {
                    OriginalFileName = details.FileName,
                    CreatedAtUtc = DateTime.UtcNow,
                    FileSize = details.FileSize,
                    Status = BookStatus.SavedToStorage,
                    UserId = details.UserId,
                    StoragePath = savingResult.Path,
                    Title = details.Title,
                    Author = details.Author                    
                };
                var metadataSavingResult = await _bookRepository.AddNewBook(newBook);
                if (!metadataSavingResult)
                {
                    var message = $"File was uploaded, but metadata wasn't saved to db. Filename: {details.FileName}";
                    _logger.LogError(message);
                    await _storageService.DeleteBookFromStorage(details.UserId, details.FileName);
                    return new ServiceResult<UploadBookResult>(
                        new UploadBookResult(null, BookStatus.Failed),
                        message);
                }
                await _eventPublisher.PublishAsync(new BookUploadedEvent(newBook.Id), token);

                return new ServiceResult<UploadBookResult>(
                    new UploadBookResult(newBook, newBook.Status),
                    null);
            }
            catch (Exception e)
            {
                var message = $"Failed to upload book file. Exception: {e.Message}";
                _logger.LogError(message);
                return new ServiceResult<UploadBookResult>(new UploadBookResult(null, BookStatus.Failed), message);
            }
        }

        public async Task<ServiceResult<IEnumerable<BookModel>>> GetByUserIdAsync(int userId, CancellationToken token)
        {
            try
            {
                var res = await _bookRepository.GetByUserIdAsync(userId, token);
                return new ServiceResult<IEnumerable<BookModel>>(res.ToDto(), null);
            }
            catch (Exception e)
            {

                return new ServiceResult<IEnumerable<BookModel>>(new List<BookModel>(),
                    $"Failed to get books for user {userId}. Exception: {e.Message}");
            }
        
        }
        public async Task<ServiceResult<Book?>> GetBookByUserAndFileNameAsync(int userId,
            string fileName,
            CancellationToken token)
        {
            try
            {
                var res = await _bookRepository.GetByUserAndFileNameAsync(userId, fileName, token);
                return new ServiceResult<Book?>(res, null);
            }
            catch (Exception e)
            {
                return new ServiceResult<Book?>(null,
                    $"Failed to get books for user {userId}. Exception: {e.Message}");
            }
        }
             

        private async Task<bool> ValidateUploadBookModel(UploadBookDetails details, CancellationToken token)
        {
            if (details.UserId == 0)
                return false;

            var existingBook = await GetBookByUserAndFileNameAsync(details.UserId, details.FileName, token);
            if (existingBook.Data != null)
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
