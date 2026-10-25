using BookReader.Core.Abstract.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookReader.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LearningController : BaseAPIController
    {
        private readonly ILearningService _learningService;
        public LearningController(ILearningService learningService)
        {
            _learningService = learningService;
        }

        [HttpPost]
        public async Task<IActionResult> AddToLearning([FromBody] string value, CancellationToken token)
        {
            var res = await _learningService.LearnWord(UserId, value, token);
            var code = res.IsSuccess ?
                StatusCodes.Status200OK :
                StatusCodes.Status400BadRequest;
            return GenerateResponse(code, res);
        }
        [HttpGet]
        public async Task<IActionResult> GetUserVocabulary(CancellationToken token)
        {
            var res = await _learningService.GetUserVocabulary(UserId, token);
            var code = res.IsSuccess ?
                StatusCodes.Status200OK :
                StatusCodes.Status400BadRequest;
            return GenerateResponse(code, res);
        }
    }
}
