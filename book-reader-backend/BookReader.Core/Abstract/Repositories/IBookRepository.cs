using BookReader.Core.Entities;

namespace BookReader.Core.Abstract.Repositories
{
    public interface IBookRepository : IRepository
    {
        Task<Book?> GetByIdAsync(int bookId, CancellationToken token);
        Task<IReadOnlyCollection<Book>> GetByUserIdAsync(int userId, CancellationToken token);
        Task<Book?> GetByUserAndFileNameAsync(int userId,string fileName, CancellationToken token);
        Task<bool> AddNewBook(Book book);
    }
}
