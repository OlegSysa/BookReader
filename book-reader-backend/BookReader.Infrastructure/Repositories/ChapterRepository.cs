using BookReader.Core.Abstract.Repositories;
using BookReader.Core.Entities;
using BookReader.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Infrastructure.Repositories
{
    public class ChapterRepository : BaseRepository, IChapterRepository
    {
        private const int COUNT_TO_ADD = 100;
        public ChapterRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<bool> Add(Chapter chapter)
        {
            _context.Chapters.Add(chapter);
            var res = await _context.SaveChangesAsync();
            return res > 0;
        }
        public async Task<bool> AddBatchAsync(IEnumerable<Chapter> chapters, CancellationToken token)
        {
            var itterationsCount = (chapters.Count() + COUNT_TO_ADD - 1) / COUNT_TO_ADD; 
            for (int i = 0; i < itterationsCount; i++)
            {
                await _context.Chapters.AddRangeAsync(chapters, token);
            }
            var res = await _context.SaveChangesAsync(token);
            return res > 0;
        }

        public async Task<Chapter?> GetReqiredChapter(int bookId, int selector, CancellationToken token) => 
            await _context.Chapters.FirstOrDefaultAsync(c => c.BookId == bookId && c.SelectorIndex == selector, token);
    }
}
