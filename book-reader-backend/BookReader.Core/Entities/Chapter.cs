using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.Entities
{
    public class Chapter
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public required int SelectorIndex { get; set; }
        public required string Content { get; set; }
        public DateTime Created { get; set; }
        public Book Book { get; set; } = null!;

    }
}
