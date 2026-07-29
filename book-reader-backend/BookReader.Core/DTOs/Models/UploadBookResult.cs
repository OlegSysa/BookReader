using BookReader.Core.Entities;
using BookReader.Core.Enums;

namespace BookReader.Core.DTOs.Models
{
    public sealed record UploadBookResult(Book? Book, BookStatus Status);
}
