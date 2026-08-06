using BookReader.Core.DTOs.Models;
using BookReader.Core.Entities;
using BookReader.Core.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookReader.Core.Abstract.Services
{
    public interface IStorageService
    {
        Task<UploadFileResult> SaveBookToStorageAsync(string storagePath, Stream stream,
            string fileName, int userId, CancellationToken token = default);
        Task<UploadFileResult> SaveParsedBookToStorageAsync(int userId,
            int bookId, string storageParsedFilesPath,
            IEnumerable<DocumentNode> data,
            CancellationToken token = default);

        Task<bool> DeleteBookFromStorage(int userId, string fileName);
        Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken = default);
    }
}
