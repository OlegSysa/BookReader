using BookReader.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.Abstract.Repositories
{
    public interface IChapterRepository
    {
        Task<bool> Add(Chapter chapter);
        Task<bool> AddBatchAsync(IEnumerable<Chapter> chapters, CancellationToken token);
        Task<Chapter?> GetReqiredChapter(int bookId, int selector, CancellationToken token);
    }
}
