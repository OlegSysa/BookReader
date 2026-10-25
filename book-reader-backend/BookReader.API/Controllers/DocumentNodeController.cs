using BookReader.Core.Abstract.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookReader.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentNodeController : BaseAPIController
    {
        private readonly IDocumentNodeService _docNodeService;
        public DocumentNodeController(IDocumentNodeService docNodeService)
        {
            _docNodeService = docNodeService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int bookId, int pageNumber, bool useBookmark, CancellationToken token)
        {
            var res = await _docNodeService.GetPageContentAsync(UserId,
                bookId,
                pageNumber,
                useBookmark,
                token);
            var statusCode = res.IsSuccess ?
                StatusCodes.Status200OK :
                StatusCodes.Status404NotFound;
            return GenerateResponse(statusCode, res);
        }
    }
}
