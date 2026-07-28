using BookReader.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.Entities
{
    public class Book
    {
        public Guid Id { get; set; }

        public required int UserId { get; set; }

        public required string OriginalFileName { get; set; }

        public required string StoragePath { get; set; }

        public long FileSize { get; set; }

        public required BookStatus Status { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}
