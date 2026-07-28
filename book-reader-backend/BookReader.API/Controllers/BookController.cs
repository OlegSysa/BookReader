using BookReader.Core.Abstract.Services;
using BookReader.Core.DTOs.Requests;
using BookReader.Core.DTOs.Responses;
using BookReader.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BookReader.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : Controller
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<IReadOnlyCollection<Book>> Get(int userId, CancellationToken token)
        {
            return await _bookService.GetByUserIdAsync(userId, token);
        }

        [HttpPost]
        public async Task<UploadBookResponse> Add([FromForm] UploadBookRequest request, CancellationToken token)
        {
            var userId = 0;//ToDo
            //ToDo Add validation file format etc
            var res = await _bookService.UploadAsync(request.File, userId, token);
            var response = new UploadBookResponse()
            {
                Code = res.Item1 ? 200 : 500,
                Success = res.Item1,
                Status = res.Item2
            };
            return response;
        }

    }
}
