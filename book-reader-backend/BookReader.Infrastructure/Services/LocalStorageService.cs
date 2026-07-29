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
            string fileName, CancellationToken token)
        {
            var storagePath = _config["Storage:BooksPath"] ?? string.Empty;
            if (string.IsNullOrEmpty(storagePath))
                return new UploadFileRawResult(BookStatus.Failed, storagePath);
            Directory.CreateDirectory(storagePath);
            var filePath = Path.Combine(storagePath, fileName);
            await using var fileStream = File.Create(filePath);
            await stream.CopyToAsync(fileStream, token);
            return new UploadFileRawResult(BookStatus.SavedToStorage, filePath);
        }
    }
}
