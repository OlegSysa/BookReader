using AngleSharp.Io;
using BookReader.API.Models.Requests;
using BookReader.API.Models.Responses;
using BookReader.Core.Abstract.Services;
using BookReader.Core.DTOs.Models;
using BookReader.Core.Entities;
using BookReader.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookReader.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : BaseAPIController
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int userId, CancellationToken token)
        {
            var res = await _bookService.GetByUserIdAsync(userId, token);
            var statusCode = res.IsSuccess ?
                StatusCodes.Status200OK :
                StatusCodes.Status404NotFound;
            return GenerateResponse(res, statusCode);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromForm] UploadBookRequest request, CancellationToken token)
        {
            var userId = 1;//ToDo
            await using var stream = request.File.OpenReadStream();
            var fileDetails = new UploadBookDetails(request.File.FileName, request.File.Length, userId);
            var res = await _bookService.UploadAsync(stream, fileDetails, token);
            var statusCode = res.IsSuccess ?
                StatusCodes.Status202Accepted :
                StatusCodes.Status400BadRequest;
            return GenerateResponse(res, statusCode);
        }
    }
}
