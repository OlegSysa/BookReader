using BookReader.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Infrastructure.Repositories
{
    public abstract class BaseRepository
    {
        protected readonly AppDbContext context;
        public BaseRepository(AppDbContext _context)
        {
            context = _context;
        }
    }
}
