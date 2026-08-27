using BookReader.Core.DTOs.Models;

namespace BookReader.Core.Abstract.Services
{
    public interface IStorageService
    {
        Task<UploadFileResult> SaveBookToStorageAsync(string storagePath, Stream stream,
            string fileName, int userId, CancellationToken token = default);
        Task<UploadFileResult> SaveParsedBookToStorageAsync(int userId,
            int bookId,
            DocumentNode data,
            CancellationToken token = default);

        Task<bool> DeleteBookFromStorage(int userId, string fileName);
        Task<bool> DeleteParsedBookFromStorage(int userId, int bookId, CancellationToken token);
        Task<Stream> GetBookAsync(string path, CancellationToken cancellationToken = default);
        Task<Stream> GetParsedBookAsync(string path, CancellationToken cancellationToken = default);
    }
}
