using BookReader.Core.Entities;

namespace BookReader.Core.Abstract.Repositories
{
    public interface IBookRepository
    {
        Task<IReadOnlyCollection<Book>> GetByUserIdAsync(int userId, CancellationToken token);
        Task<bool> AddNewBook(Book book);
    }
}
