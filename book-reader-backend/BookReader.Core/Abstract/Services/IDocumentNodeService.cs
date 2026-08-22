using BookReader.Core.DTOs.Models;
using BookReader.Core.Entities;
namespace BookReader.Core.Abstract.Services
{
    public interface IDocumentNodeService
    {
        Task<ServiceResult<ChapterViewResult>> GetRequiredChapterAsync(int bookId,
            int index,
            int pageNumber,
            CancellationToken token);
    }
}
