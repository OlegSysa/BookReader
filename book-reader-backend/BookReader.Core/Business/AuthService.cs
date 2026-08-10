using AngleSharp.Io;
using BookReader.Core.Abstract.Repositories;
using BookReader.Core.Abstract.Services;
using BookReader.Core.DTOs.Models;
using BookReader.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookReader.Core.Business
{
    public class AuthService : BaseService<AuthService>, IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        public AuthService(IUserRepository userRepository,
            IPasswordHasher<User> hasher,
            IConfiguration config,
            ILogger<AuthService> logger) : base(config, logger)
        {
            _userRepository = userRepository;
            _passwordHasher = hasher;
        }

        public async Task<ServiceResult<string>> RegisterAsync(string email, string pass, CancellationToken token)
        {
            var exists = await _userRepository.GetAsync(email, token);

            if (exists != null)
                return new ServiceResult<string>(null, "User already exists");

            var user = new User
            {
                Email = email,
                RoleId = 1
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, pass);
            await _userRepository.AddAsync(user);
            var saved = await _userRepository.GetAsync(user.Email, token);
            if (saved == null)
            {
                return new ServiceResult<string>(null, "Failed to register new user");
            }
            var jwt = GenerateToken(saved);
            return new ServiceResult<string>(jwt, null);
        }

        public async Task<ServiceResult<string>> LoginAsync(string email, string pass, CancellationToken token)
        {
            var user = await _userRepository.GetAsync(email, token);
            if (user == null)
                return new ServiceResult<string>(null, "Invalid credentials");

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, pass);
            if (result == PasswordVerificationResult.Failed)
                return new ServiceResult<string>(null, "Invalid credentials");

            var jwt = GenerateToken(user);
            return new ServiceResult<string>(jwt, null);
        }

        public string GenerateToken(User user)
        {
            var jwt = _config.GetSection("Jwt");
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.Name)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    int.Parse(jwt["ExpiresMinutes"]!)),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
