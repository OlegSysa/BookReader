using AngleSharp.Io;
using BookReader.API.Models.Responses;
using BookReader.Core.Abstract.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookReader.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TranslationController : BaseAPIController
    {
        private readonly ITranslationService _translationService;
        public TranslationController(ITranslationService translationService)
        {
            _translationService = translationService;
        }

        [HttpGet]
        public async Task<IActionResult> Translate(string value, CancellationToken token)
        {
            var res = await _translationService.TranslateAsync(value, token);
            var statusCode = res.IsSuccess ?
                StatusCodes.Status200OK :
                StatusCodes.Status404NotFound;
            return GenerateResponse(res, statusCode);
        }

        [HttpGet]
        [Route("sentence-translation")]
        public async Task<IActionResult> TranslateSentence(int sentenceId, string value, CancellationToken token)
        {
            var res = await _translationService.TranslateSentenceAsync(sentenceId, value, token);
            var statusCode = res.IsSuccess ?
                StatusCodes.Status200OK :
                StatusCodes.Status404NotFound;
            return GenerateResponse(res, statusCode);
        }
    }
}
