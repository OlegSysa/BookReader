using BookReader.Core.Abstract.Repositories;
using BookReader.Core.Entities;
using BookReader.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Infrastructure.Repositories
{
    public class BookRepository : BaseRepository, IBookRepository
    {
        public BookRepository(AppDbContext context) : base (context)
        {  
        }
        public async Task<bool> AddNewBook(Book book)
        {
            _context.Books.Add(book);
            var res = await _context.SaveChangesAsync();
            return res > 0;
        }

        public async Task<IReadOnlyCollection<Book>> GetByUserIdAsync(int userId, CancellationToken token) => 
            await _context.Books.Where(b => b.UserId == userId).ToListAsync(token);

        public async Task<Book?> GetByUserAndFileNameAsync(int userId, string fileName, CancellationToken token) =>
            await _context.Books.FirstOrDefaultAsync(b => b.UserId == userId && b.OriginalFileName == fileName, token);

        public Task<Book?> GetByIdAsync(int bookId, CancellationToken token) => 
            _context.Books.FirstOrDefaultAsync(b => b.Id == bookId, token);
    }
}
