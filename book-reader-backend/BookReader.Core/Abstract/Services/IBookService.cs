using BookReader.Core.DTOs.Models;
using BookReader.Core.Entities;

namespace BookReader.Core.Abstract.Services
{
    public interface IBookService
    {
        Task<ServiceResult<IEnumerable<BookModel>>> GetByUserIdAsync(int userId, CancellationToken token);
        Task<ServiceResult<Book?>> GetBookByUserAndFileNameAsync(int userId, string fileName, CancellationToken token);
        Task<ServiceResult<UploadBookResult>> UploadAsync(Stream stream,
            UploadBookDetails details,
            CancellationToken token);
    }
}
