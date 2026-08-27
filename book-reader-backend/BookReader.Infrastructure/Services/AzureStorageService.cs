using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
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
        //private readonly string _booksContainerName;
        //private readonly string _parsedBooksContainerName;

        public AzureStorageService(IOptions<AzureStorageOptions> options)
        {
            var settings = options.Value;
            var serviceClient = new BlobServiceClient(settings.ConnectionString);

            _booksContainer = serviceClient.GetBlobContainerClient(settings.BooksContainer);
            _parsedBooksContainer = serviceClient.GetBlobContainerClient(settings.ParsedBooksContainer);
            //_booksContainerName = settings.BooksContainer;
            //_parsedBooksContainerName = settings.ParsedBooksContainer;
        }

        public async Task<bool> DeleteBookFromStorage(int userId, string fileName)
        {
            var virtualFilePath = $"{userId}/{fileName}";
            var blobClient = _booksContainer.GetBlobClient(virtualFilePath);
            await blobClient.DeleteIfExistsAsync();
            return true;
        }

        public async Task<bool> DeleteParsedBookFromStorage(int userId, int bookId, CancellationToken token)
        {
            var virtualFilePath = $"{userId}/{bookId}";
            var prefix = $"{userId}/{bookId}/";
            var chaptersBlobs = _parsedBooksContainer.GetBlobsAsync(
                traits: BlobTraits.None,
                states: BlobStates.None,
                prefix: virtualFilePath,
                cancellationToken: token);
            await foreach (var blob in chaptersBlobs)
            {
                await _parsedBooksContainer.DeleteBlobIfExistsAsync(blob.Name);
            }

            return true;
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
            DocumentNode data,
            CancellationToken token = default)
        {
            var path = Path.Combine(userId.ToString(), bookId.ToString());
            try
            {
                var filePath = $"{path}.json";
                await using var stream = new MemoryStream();

                await JsonSerializer.SerializeAsync(
                    stream,
                    data,
                    new JsonSerializerOptions { WriteIndented = true },
                    token);
                stream.Position = 0;
                var blobClient = _parsedBooksContainer.GetBlobClient(filePath);
                var res = await blobClient.UploadAsync(stream, overwrite: false, cancellationToken: token);

                return new UploadFileResult(BookStatus.SavedToStorage, filePath);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
