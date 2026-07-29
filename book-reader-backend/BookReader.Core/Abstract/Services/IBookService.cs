using BookReader.Core.DTOs.Models;
using BookReader.Core.Entities;

namespace BookReader.Core.Abstract.Services
{
    public interface IBookService
    {
        Task<IReadOnlyCollection<Book>> GetByUserIdAsync(int userId, CancellationToken token);
        Task<UploadBookResult> UploadAsync(Stream stream,
            string fileName,
            long fileSize,
            int userId,
            CancellationToken token);
    }
}
