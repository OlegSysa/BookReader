using BookReader.Core.DTOs.Models;
using BookReader.Core.Entities;
namespace BookReader.Core.Abstract.Services
{
    public interface IChapterService
    {
        Task<ServiceResult<Chapter>> GetRequiredChapterAsync(int bookId, int selector, CancellationToken token);
    }
}
