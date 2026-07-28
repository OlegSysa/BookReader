using BookReader.Core.Abstract.Repositories;
using BookReader.Core.Abstract.Services;
using BookReader.Core.Entities;
using BookReader.Core.Enums;
using Microsoft.AspNetCore.Http;

namespace BookReader.Core.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository bookRepository;
        private readonly IStorageService storageService;
        public BookService(IBookRepository _bookRepository,
            IStorageService _storageService)
        {
            bookRepository = _bookRepository;
            storageService = _storageService;
        }
        public async Task<(bool, BookStatus)> UploadAsync(IFormFile file, int userId, CancellationToken token) 
        {
            try
            {
                var saveToStorageStatus = await storageService.SaveBookToStorageAsync(file, token);
                if (saveToStorageStatus == BookStatus.Failed)
                    return (false, saveToStorageStatus);

                var storagePath = string.Empty;
                var newBook = new Book() {
                    OriginalFileName = file.FileName,
                    CreatedAtUtc = DateTime.UtcNow,
                    FileSize = file.Length,
                    Status = BookStatus.Processing,
                    UserId = userId,
                    StoragePath = storagePath
                };

                var metadataResult = await bookRepository.AddNewBook(newBook);
                return (metadataResult, newBook.Status);
            }
            catch (Exception e)
            {
                //Add logs!!!
                return (false, BookStatus.Failed);
            }
        } 

        public async Task<IReadOnlyCollection<Book>> GetByUserIdAsync(int userId, CancellationToken token) =>
             await bookRepository.GetByUserIdAsync(userId, token);

    }
}
