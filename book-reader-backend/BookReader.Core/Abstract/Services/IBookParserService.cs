using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.Abstract.Services
{
    public interface IBookParserService
    {
        Task<bool> ParseBook(int bookId, CancellationToken token);
    }
}
