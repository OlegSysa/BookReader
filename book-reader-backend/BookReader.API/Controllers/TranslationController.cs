using AngleSharp.Io;
using BookReader.API.Models.Responses;
using BookReader.Core.Abstract.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookReader.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TranslationController : Controller
    {
        private readonly ITranslationService _translationService;
        public TranslationController(ITranslationService translationService)
        {
            _translationService = translationService;
        }

        [HttpGet]
        public async Task<ApiResponse<string>> Translate(string value, CancellationToken token)
        {
            var res = await _translationService.TranslateAsync(value, token);
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
