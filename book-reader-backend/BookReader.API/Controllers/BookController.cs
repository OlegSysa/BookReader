using BookReader.API.Models.Requests;
using BookReader.Core.Abstract.Services;
using BookReader.Core.DTOs.Models;
using BookReader.Core.Entities;
using BookReader.Core.Enums;
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
            var userId = 1;//ToDo
            
            await using var stream = request.File.OpenReadStream();

            var fileDetails = new UploadBookDetails(request.File.FileName, request.File.Length, userId);
            var res = await _bookService.UploadAsync(stream, fileDetails, token);
            var response = new UploadBookResponse()
            {
                Code = res.Status != BookStatus.Failed ? 200 : 500,
                Success = res.Status != BookStatus.Failed,
                Status = res.Status
            };
            return response;
        }

    }
}
