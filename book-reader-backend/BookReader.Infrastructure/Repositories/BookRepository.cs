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
        public async Task<bool> AddNewBookAsync(Book book)
        {
            _context.Books.Add(book);
            var res = await _context.SaveChangesAsync();
            return res > 0;
        }

        public async Task<IReadOnlyCollection<Book>> GetByUserIdAsync(int userId, CancellationToken token) => 
            await _context.Books.Where(b => b.UserId == userId && !b.Deleted).ToListAsync(token);

        public async Task<Book?> GetByUserAndFileNameAsync(int userId, string fileName, CancellationToken token) =>
            await _context.Books.FirstOrDefaultAsync(b =>!b.Deleted && b.UserId == userId && b.OriginalFileName == fileName, token);

        public Task<Book?> GetByIdAsync(int bookId, CancellationToken token) => 
            _context.Books.FirstOrDefaultAsync(b =>!b.Deleted && b.Id == bookId, token);

        public async Task<bool> DeleteBookAsync(int bookId)
        {
            var entity = await _context.Books.FindAsync(bookId);
            if (entity == null)
                return false;
           
            _context.Books.Remove(entity);
            await _context.SaveChangesAsync(); 
            return true;
        }

       
    }
}
