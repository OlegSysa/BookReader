using BookReader.Core.Abstract.Repositories;
using BookReader.Core.Entities;
using BookReader.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }

        public async Task AddAsync(User user)
        {
           _context.Users.Add(user);
           await _context.SaveChangesAsync();
        }

        public async Task<User?> GetAsync(string email, CancellationToken token) =>
            await _context.Users.Include(x => x.Role).FirstOrDefaultAsync(u=> u.Email == email, token);
    }
}
