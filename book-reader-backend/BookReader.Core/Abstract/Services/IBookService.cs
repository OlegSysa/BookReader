using BookReader.Core.DTOs.Models;
using BookReader.Core.Entities;

namespace BookReader.Core.Abstract.Services
{
    public interface IBookService
    {
        Task<IReadOnlyCollection<Book>> GetByUserIdAsync(int userId, CancellationToken token);
        Task<IReadOnlyCollection<Book>> GetBookByUserAndFileNameAsync(int userId, string fileName, CancellationToken token);
        Task<UploadBookResult> UploadAsync(Stream stream,
            UploadBookDetails details,
            CancellationToken token);
    }
}
