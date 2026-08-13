using BookReader.Core.DTOs.Models;
using BookReader.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.Extensions.Mappings
{
    public static class BookMappingExtensions
    {
        public static BookModel ToDto(this Book book)
        {
            return new BookModel(
                book.Id,
                book.OriginalFileName,
                book.FileSize,
                book.Status,
                book.CreatedAtUtc);
        }

        public static IEnumerable<BookModel> ToDto(
            this IEnumerable<Book> books)
        {
            return books.Select(x => x.ToDto());
        }
    }
}
