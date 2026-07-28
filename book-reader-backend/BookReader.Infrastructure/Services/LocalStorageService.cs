using BookReader.Core.Abstract.Services;
using BookReader.Core.Business;
using BookReader.Core.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Infrastructure.Services
{
    public class LocalStorageService : BaseService, IStorageService
    {
        public LocalStorageService(IConfiguration _config) : base(_config)
        {
        }
        public async Task<BookStatus> SaveBookToStorageAsync(IFormFile file, CancellationToken token)
        {
            var storagePath = config["Storage:BooksPath"] ?? string.Empty;
            if (string.IsNullOrEmpty(storagePath))
                return BookStatus.Failed;
            var path = Path.Combine(storagePath, file.FileName);
            await using var stream = File.Create(path);
            await file.CopyToAsync(stream, token);
            return BookStatus.Uploaded;
        }
    }
}
