using BookReader.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public required int UserId { get; set; }
        public required string OriginalFileName { get; set; }
        public required string Title { get; set; }
        public required string Author { get; set; }
        public required string StoragePath { get; set; }
        public string? ParsedFilesPath { get; set; }
        public long FileSize { get; set; }
        public int ChaptersCount { get; set; }
        public required BookStatus Status { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
