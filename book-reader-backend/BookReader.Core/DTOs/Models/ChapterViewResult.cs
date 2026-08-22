using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.DTOs.Models
{
    public record ChapterViewResult
    {
        public int Index { get; set; }
        public bool IsLastChapter { get; set; }
        public int NumberOfPages { get; set; }
        public bool IsLastPage { get; set; }
        public required string Content { get; set; }

    }
}
