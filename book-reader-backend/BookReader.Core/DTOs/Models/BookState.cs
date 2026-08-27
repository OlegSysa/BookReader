using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.DTOs.Models
{
    public class BookState
    {
        public int BookId { get; set; }
        //public bool IsLastChapter { get; set; }
        public int NumberOfPages { get; set; }
        public bool IsLastPage { get; set; }
        public int Index { get; set; }
        //public int ChaptersCount { get; set; }
        public Dictionary<int, BookPage> Pages { get; set; } = new Dictionary<int, BookPage>();
    }
}
