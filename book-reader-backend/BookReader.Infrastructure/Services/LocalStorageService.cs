using BookReader.Core.Abstract.Services;
using BookReader.Core.Business;
using BookReader.Core.DTOs.Models;
using BookReader.Core.Entities;
using BookReader.Core.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BookReader.Infrastructure.Services
{
    public class LocalStorageService : BaseService<LocalStorageService>, IStorageService
    {
        public LocalStorageService(IConfiguration _config,
            ILogger<LocalStorageService> logger) : base(_config, logger)
        {
        }
        public async Task<UploadFileResult> SaveBookToStorageAsync(string storagePath, Stream stream, 
            string fileName,int userId,CancellationToken token)
        {
            var storageUserPath = Path.Combine(storagePath, userId.ToString());
            if (string.IsNullOrEmpty(storageUserPath))
                return new UploadFileResult(BookStatus.Failed, storageUserPath);
            if (!Directory.Exists(storageUserPath))
            {
                Directory.CreateDirectory(storageUserPath);
            }
            var filePath = Path.Combine(storageUserPath, fileName);
            await using var fileStream = File.Create(filePath);
            await stream.CopyToAsync(fileStream, token);
            return new UploadFileResult(BookStatus.SavedToStorage, filePath);
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

        public async Task<UploadFileResult> SaveParsedBookToStorageAsync(int userId, int bookId,
            string storageParsedFilesPath,
            IEnumerable<DocumentNode> data,
            CancellationToken token)
        {
            var bookPath = Path.Combine(storageParsedFilesPath, userId.ToString(), bookId.ToString());
            if (!Directory.Exists(bookPath))
            {
                Directory.CreateDirectory(bookPath);
            }
            foreach (var (chapter, index) in data.Select((chapter, index) => (chapter, index)))
            {
                var fileName = Path.Combine(bookPath, $"{index + 1}.json");
 
                await using var stream = File.Create(fileName);
                await JsonSerializer.SerializeAsync(stream, chapter, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            }
            return new UploadFileResult(BookStatus.Ready, bookPath);
        }

        public Task<Stream> GetBookAsync(string path, CancellationToken cancellationToken = default)  => 
            Task.FromResult<Stream>(File.OpenRead(path));
        public Task<Stream> GetParsedBookAsync(string path, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public async Task<bool> DeleteParsedBookFromStorage(int userId, int bookId)
        {
            var storagePath = _config["Storage:ParsedBooksPath"] ?? string.Empty;
            if (string.IsNullOrEmpty(storagePath) || !Directory.Exists(storagePath))
            {
                _logger.LogError($"Failed to delete book file. Can not find storage path.");
                return false;
            }
            var dirPath = Path.Combine(storagePath, userId.ToString(), bookId.ToString());
            if (Directory.Exists(dirPath))
            {
                Directory.Delete(dirPath, true);
            }
            return true;
        }
    }
}
