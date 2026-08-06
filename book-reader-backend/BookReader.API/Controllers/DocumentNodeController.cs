using BookReader.API.Models.Responses;
using BookReader.Core.Abstract.Services;
using BookReader.Core.DTOs.Models;
using BookReader.Core.Entities;
using BookReader.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookReader.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentNodeController : Controller
    {
        private readonly IDocumentNodeService _docNodeService;
        public DocumentNodeController(IDocumentNodeService docNodeService)
        {
            _docNodeService = docNodeService;
        }

        [HttpGet]
        public async Task<ApiResponse<string>> Get(int bookId, int selector, CancellationToken token)
        {
            var res = await _docNodeService.GetRequiredChapterAsync(bookId, selector, token);
            return new ApiResponse<string>()
            {
                Data = res.Data,
                Code = res.Data != null ? 200 : 500,
                Success = res.Data != null,
                ErrorMessage = res.Error
            };
        }
    }
}
