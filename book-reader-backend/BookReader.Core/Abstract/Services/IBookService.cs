using BookReader.Core.Entities;
using BookReader.Core.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.Abstract.Services
{
    public interface IBookService
    {
        Task<IReadOnlyCollection<Book>> GetByUserIdAsync(int userId, CancellationToken token);
        Task<(bool, BookStatus)> UploadAsync(IFormFile file, int userId, CancellationToken token);
    }
}
