using BookReader.API.Extensions;
using BookReader.API.Models.Requests;
using BookReader.API.Models.Responses;
using BookReader.Core.Abstract.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookReader.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : BaseAPIController
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request, CancellationToken token)
        {
            Thread.Sleep(5000);
            var tokenResult = await _authService.RegisterAsync(request.Email, request.Password, token);
            if (tokenResult.IsSuccess)
            {
                Response.SetTokenCookie(tokenResult.Data!);
                tokenResult.Data = "success";
            }

            var statusCode = tokenResult.IsSuccess ? 
                StatusCodes.Status201Created :
                StatusCodes.Status409Conflict;
            return GenerateResponse(statusCode, tokenResult);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request, CancellationToken token)
        {
            var tokenResult = await _authService.LoginAsync(request.Email, request.Password, token);
            if (tokenResult.IsSuccess)
            {
                Response.SetTokenCookie(tokenResult.Data!);
                tokenResult.Data = "success";
            }
            var statusCode = tokenResult.IsSuccess ? 
                StatusCodes.Status200OK :
                StatusCodes.Status401Unauthorized;
            return GenerateResponse(statusCode, tokenResult);
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("access_token", new CookieOptions
            {
                Path = "/",
                Secure = true,
                SameSite = SameSiteMode.None
            });
            return Ok();
        }
    }
}
