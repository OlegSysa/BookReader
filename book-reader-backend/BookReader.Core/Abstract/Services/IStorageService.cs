using BookReader.Core.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.Abstract.Services
{
    public interface IStorageService
    {
        Task<BookStatus> SaveBookToStorageAsync(IFormFile file, CancellationToken token);
    }
}
