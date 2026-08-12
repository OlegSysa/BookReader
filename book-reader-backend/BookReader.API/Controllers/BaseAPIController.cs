using BookReader.API.Models.Responses;
using BookReader.Core.DTOs.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookReader.API.Controllers
{
    public abstract class BaseAPIController : ControllerBase
    {
        protected IActionResult GenerateResponse<T>(ServiceResult<T> result, int code)
        {
            var isSuccess = string.IsNullOrEmpty(result.Error);
            var respose =  new ApiResponse<T>()
            {
                Data = result.Data,
                Code = code,
                Success = isSuccess,
                ErrorMessage = result.Error
            };

            return StatusCode(code, respose);
        }
    }
}
