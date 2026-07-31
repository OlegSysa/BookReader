using BookReader.API.Models.Responses;
using BookReader.Core.Abstract.Services;
using BookReader.Core.Entities;
using BookReader.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookReader.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChapterController : Controller
    {
        private readonly IChapterService _chapterService;
        public ChapterController(IChapterService chapterService)
        {
            _chapterService = chapterService;
        }

        [HttpGet]
        public async Task<GetChapterResponse> Get(int bookId, int selector, CancellationToken token)
        {
            var res = await _chapterService.GetRequiredChapterAsync(bookId, selector, token);
            return new GetChapterResponse()
            {
                Chapter = res.Data,
                Code = res.Data != null ? 200 : 500,
                Success = res.Data != null,
                ErrorMessage = res.Error
            };
        }

    }
}
