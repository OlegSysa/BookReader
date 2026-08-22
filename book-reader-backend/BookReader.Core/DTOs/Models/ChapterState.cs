using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.DTOs.Models
{
    public class ChapterState
    {
        public int BookId { get; set; }
        public bool IsLastChapter { get; set; }
        public int NumberOfPages { get; set; }
        public bool IsLastPage { get; set; }
        public int Index { get; set; }
        public Dictionary<int, Page> Pages { get; set; } = new Dictionary<int, Page>();
    }
}
