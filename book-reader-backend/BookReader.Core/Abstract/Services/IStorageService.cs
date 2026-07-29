using BookReader.Core.DTOs.Models;
using BookReader.Core.Enums;

namespace BookReader.Core.Abstract.Services
{
    public interface IStorageService
    {
        Task<UploadFileRawResult> SaveBookToStorageAsync(Stream stream,
            string fileName, int userId, CancellationToken token);

        Task<bool> DeleteBookFromStorage(int userId, string fileName);
    }
}
