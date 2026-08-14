using BookReader.API.Extensions;
using BookReader.API.Models.Requests;
using BookReader.API.Models.Responses;
using BookReader.Core.Abstract.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookReader.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : BaseAPIController
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _config;
        public AuthController(IConfiguration config, IAuthService authService)
        {
            _authService = authService;
            _config = config;
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

        [HttpGet("google")]
        public IActionResult GoogleLogin(string mode)
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = $"/api/auth/google-callback-{mode}"
            };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-callback-login")]
        public async Task<IActionResult> GoogleCallbackLogin(CancellationToken token)
        {
            var result = await HttpContext.AuthenticateAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded)
                return Unauthorized();

            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var externalId = result.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(externalId))
                return Unauthorized();

            var res = await _authService.LoginExternalAsync(email, externalId, token);
            if (res.IsSuccess)
            {
                Response.SetTokenCookie(res.Data!);
            }
            var appUrl = _config["App:BaseUrl"];
            return Redirect($"{appUrl}/dashboard");
        }

        [HttpGet("google-callback-register")]
        public async Task<IActionResult> GoogleCallbackRegister(CancellationToken token)
        {
            var result = await HttpContext.AuthenticateAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded)
                return Unauthorized();

            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var externalId = result.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(externalId))
                return Unauthorized();

            var res = await _authService.RegisterExternalAsync(email, externalId, token);
            if (res.IsSuccess)
            {
                Response.SetTokenCookie(res.Data!);
            }
            var appUrl = _config["App:BaseUrl"];
            return Redirect($"{appUrl}/dashboard");
        }
    }
}
