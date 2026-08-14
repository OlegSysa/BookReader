using BookReader.Core.DTOs.Models;
using BookReader.Core.Entities;
using Microsoft.AspNetCore.Identity.Data;

namespace BookReader.Core.Abstract.Services
{
    public interface IAuthService
    {
        string GenerateToken(User user);
        Task<ServiceResult<string>> RegisterAsync(string email, string pass, CancellationToken token);
        Task<ServiceResult<string>> RegisterExternalAsync(string email, string pass, CancellationToken token);
        Task<ServiceResult<string>> LoginAsync(string email, string userIdentifier, CancellationToken token);
        Task<ServiceResult<string>> LoginExternalAsync(string email, string userIdentifier, CancellationToken token);
    }
}
