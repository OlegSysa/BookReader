using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.Abstract.Repositories
{
    public interface IRepository
    {
        Task SaveChangesAsync();
        //Task ExecuteInTransactionAsync(Func<CancellationToken, Task<bool>> func);
    }
}
