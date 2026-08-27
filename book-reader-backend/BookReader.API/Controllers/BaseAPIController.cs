using BookReader.API.Models.Responses;
using BookReader.Core.DTOs.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookReader.API.Controllers
{
    public abstract class BaseAPIController : ControllerBase
    {
        protected IActionResult GenerateResponse<T>( int code, ServiceResult<T>? result = null)
        {
            if (result == null)
                result = new ServiceResult<T>(default, null);
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

        protected int UserId { get { 
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdClaim, out var userId))
                    return 0;
                else return userId;
            } }
    }
}
