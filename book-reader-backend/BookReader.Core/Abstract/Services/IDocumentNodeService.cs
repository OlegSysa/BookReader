using BookReader.Core.DTOs.Models;
using BookReader.Core.Entities;
namespace BookReader.Core.Abstract.Services
{
    public interface IDocumentNodeService
    {
        Task<ServiceResult<BookViewResult>> GetPageContentAsync(int userId, int bookId,
            int pageNumber,
            CancellationToken token);
    }
}
