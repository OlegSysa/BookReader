using BookReader.API.Models.Responses;
using BookReader.Core.DTOs.Models;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace BookReader.API.Extensions
{
    public static class HttpExtensions
    {
        public static void SetTokenCookie(this HttpResponse response, IWebHostEnvironment environment, string value, int? expireTime = null)
        {
            if (string.IsNullOrEmpty(value))
                return;
            var options = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddMinutes(60)
            };
            if (!environment.IsDevelopment())
            {
                options.Domain = ".bookly.world";
            }

            response.Cookies.Append("access_token", value,options );
        }

        
    }
}
