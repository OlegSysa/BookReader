using BookReader.Infrastructure.Persistence;
namespace BookReader.Infrastructure.Repositories
{
    public abstract class BaseRepository
    {
        protected readonly AppDbContext _context;
        public BaseRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

        //public  Task<bool> ExecuteInTransactionAsync(CancellationToken token, Func<CancellationToken, Task<bool>> func)
        //{
        //     using var transaction = _context.Database.BeginTransactionAsync();

        //    try
        //    {
        //        var res = func(token);
        //        transaction.CommitAsync();
        //        return res;
        //    }
        //    catch
        //    {
        //        await transaction.RollbackAsync();
        //        throw;
        //    }
           
        //}
    }
}
