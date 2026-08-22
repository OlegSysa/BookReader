using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.DTOs.Models
{
    public class Page
    {
        public int Number { get; set; }
        public required List<DocumentNode> Paragraphs { get; set; }
    }
}
