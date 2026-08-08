using Azure.Storage.Blobs;
using BookReader.Core.Abstract.Services;
using BookReader.Core.DTOs.Models;
using BookReader.Core.Enums;
using BookReader.Infrastructure.Persistence.Configurations;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace BookReader.Infrastructure.Services
{
    public class AzureStorageService : IStorageService
    {
        private readonly BlobContainerClient _booksContainer;
        private readonly BlobContainerClient _parsedBooksContainer;
        private readonly string _booksContainerName;
        private readonly string _parsedBooksContainerName;

        public AzureStorageService(IOptions<AzureStorageOptions> options)
        {
            var settings = options.Value;
            var serviceClient = new BlobServiceClient(settings.ConnectionString);

            _booksContainer = serviceClient.GetBlobContainerClient(settings.BooksContainer);
            _parsedBooksContainer = serviceClient.GetBlobContainerClient(settings.ParsedBooksContainer);
            _booksContainerName = settings.BooksContainer;
            _parsedBooksContainerName = settings.ParsedBooksContainer;
        }

        public async Task<bool> DeleteBookFromStorage(int userId, string fileName)
        {
            var virtualFilePath = $"{userId}/{fileName}";
            var blobClient = _booksContainer.GetBlobClient(virtualFilePath);
            var res = await blobClient.DeleteIfExistsAsync();
            return res.Value;
        }

        public async Task<bool> DeleteParsedBookFromStorage(int userId, int bookId)
        {
            var virtualFilePath = $"{userId}/{bookId}";
            var blobClient = _booksContainer.GetBlobClient(virtualFilePath);
            var res = await blobClient.DeleteIfExistsAsync();
            return res.Value;
        }

        public async Task<Stream> GetBookAsync(string path, CancellationToken cancellationToken = default)
        {
            var blobClient = _booksContainer.GetBlobClient(path);
            return await blobClient.OpenReadAsync(cancellationToken: cancellationToken);
        }

        public async Task<Stream> GetParsedBookAsync(string path, CancellationToken cancellationToken = default)
        {
            var blobClient = _parsedBooksContainer.GetBlobClient(path);
            return await blobClient.OpenReadAsync(cancellationToken: cancellationToken);
        }

        public async Task<UploadFileResult> SaveBookToStorageAsync(string storagePath,
            Stream stream,
            string fileName,
            int userId,
            CancellationToken token = default)
        {
            var virtualFilePath = $"{userId}/{fileName}";
            var blobClient = _booksContainer.GetBlobClient(virtualFilePath);
            var res = await blobClient.UploadAsync(stream, overwrite: false, cancellationToken: token);
            return new UploadFileResult(BookStatus.SavedToStorage, virtualFilePath);
        }

        public async Task<UploadFileResult> SaveParsedBookToStorageAsync(int userId,
            int bookId,
            string storageParsedFilesPath,
            IEnumerable<DocumentNode> data,
            CancellationToken token = default)
        {
            var virtualDirPath = Path.Combine(userId.ToString(), bookId.ToString());
            var tempDirName = Path.Combine(storageParsedFilesPath, Guid.NewGuid().ToString());
            if(!Directory.Exists(tempDirName))
            {
                Directory.CreateDirectory(tempDirName);
            }
            
            foreach (var (chapter, index) in data.Select((chapter, index) => (chapter, index)))
            {
                var fileName = $"{index + 1}.json";
                var tempFilePath = Path.Combine(tempDirName, fileName);
                await using var stream = File.Create(tempFilePath);
                await JsonSerializer.SerializeAsync(stream, chapter, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                var azureBlobFilePath = Path.Combine(virtualDirPath, fileName);
                var blobClient = _parsedBooksContainer.GetBlobClient(azureBlobFilePath);
                stream.Position = 0;
                var res = await blobClient.UploadAsync(stream, overwrite: false, cancellationToken: token);
            }
            if (Directory.Exists(tempDirName))
            {
                Directory.Delete(tempDirName, true);
            }

            return new UploadFileResult(BookStatus.SavedToStorage, virtualDirPath);
        }
    }
}
