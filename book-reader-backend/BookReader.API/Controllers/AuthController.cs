using BookReader.API.Models.Requests;
using BookReader.API.Models.Responses;
using BookReader.Core.Abstract.Services;
using BookReader.Core.Business;
using BookReader.Core.Entities;
using BookReader.Core.Enums;
using Microsoft.AspNetCore.Mvc;

namespace BookReader.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        [HttpPost("register")]
        public async Task<ApiResponse<string>> Register(RegisterRequest request, CancellationToken token)
        {
            var tokenResult = await _authService.RegisterAsync(request.Email, request.Password, token);

            return new ApiResponse<string>()
            {
                Data = tokenResult.Data,
                Code = string.IsNullOrEmpty(tokenResult.Error) ? 200 : 500,
                Success = string.IsNullOrEmpty(tokenResult.Error),
                ErrorMessage = tokenResult.Error
            };
        }

        [HttpPost("login")]
        public async Task<ApiResponse<string>> Login(LoginRequest request, CancellationToken token)
        {
            var tokenResult = await _authService.LoginAsync(request.Email, request.Password, token);

            return new ApiResponse<string>()
            {
                Data = tokenResult.Data,
                Code = string.IsNullOrEmpty(tokenResult.Error) ? 200 : 401,
                Success = string.IsNullOrEmpty(tokenResult.Error),
                ErrorMessage = tokenResult.Error
            };
        }
    }
}
