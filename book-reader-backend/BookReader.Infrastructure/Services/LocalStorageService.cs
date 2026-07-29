using BookReader.Core.Abstract.Services;
using BookReader.Core.Business;
using BookReader.Core.DTOs.Models;
using BookReader.Core.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BookReader.Infrastructure.Services
{
    public class LocalStorageService : BaseService<LocalStorageService>, IStorageService
    {
        public LocalStorageService(IConfiguration _config,
            ILogger<LocalStorageService> logger) : base(_config, logger)
        {
        }
        public async Task<UploadFileRawResult> SaveBookToStorageAsync(Stream stream, 
            string fileName,int userId,CancellationToken token)
        {
            var storageRootPath = _config["Storage:BooksPath"] ?? string.Empty;
            var storageUserPath = Path.Combine(storageRootPath, userId.ToString());
            if (string.IsNullOrEmpty(storageUserPath))
                return new UploadFileRawResult(BookStatus.Failed, storageUserPath);
            if (!Directory.Exists(storageUserPath))
            {
                Directory.CreateDirectory(storageUserPath);
            }
            var filePath = Path.Combine(storageUserPath, fileName);
            await using var fileStream = File.Create(filePath);
            await stream.CopyToAsync(fileStream, token);
            return new UploadFileRawResult(BookStatus.SavedToStorage, filePath);
        }

        public async Task<bool> DeleteBookFromStorage(int userId, string fileName)
        {
            var storagePath = _config["Storage:BooksPath"] ?? string.Empty;
            if (string.IsNullOrEmpty(storagePath) || !Directory.Exists(storagePath))
            {
                _logger.LogError($"Failed to delete book file. Can not find storage path.");
                return false;
            }
            var filePath = Path.Combine(storagePath, userId.ToString(), fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            return true;
        }
    }
}
