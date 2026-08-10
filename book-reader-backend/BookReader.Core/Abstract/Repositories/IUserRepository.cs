using BookReader.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.Abstract.Repositories
{
    public interface IUserRepository : IRepository
    {
        Task<User?> GetAsync(string email, CancellationToken token);
        Task AddAsync(User user);
    }
}
