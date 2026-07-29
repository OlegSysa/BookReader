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
            context.Books.Add(book);
            var res = await context.SaveChangesAsync();
            return res > 0;
        }

        public async Task<IReadOnlyCollection<Book>> GetByUserIdAsync(int userId, CancellationToken token) => 
            await context.Books.Where(b => b.UserId == userId).ToListAsync(token);

        public async Task<IReadOnlyCollection<Book>> GetByUserAndFileNameAsync(int userId, string fileName, CancellationToken token) =>
            await context.Books.Where(b => b.UserId == userId && b.OriginalFileName == fileName).ToListAsync(token);


    }
}
