using BookReader.Core.Abstract.Repositories;
using BookReader.Core.Abstract.Services;
using BookReader.Core.DTOs.Models;
using BookReader.Core.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace BookReader.Core.Business
{
    public class ChapterService : BaseService<ChapterService>, IChapterService
    {
        private readonly IChapterRepository _chapterRepository;
        public ChapterService(IChapterRepository chapterRepository,
            IConfiguration config,
            ILogger<ChapterService> logger) : base(config, logger)
        {
            _chapterRepository = chapterRepository;
        }

        public async Task<ServiceResult<Chapter>> GetRequiredChapterAsync(int bookId, int selector, CancellationToken token)
        {
            var res = await _chapterRepository.GetReqiredChapter(bookId, selector, token);
            return new ServiceResult<Chapter>(res,
                res != null 
                ? string.Empty 
                : $"Cannot find chapter. BookId:{bookId}, Selector: {selector}");
        }

    }
}
