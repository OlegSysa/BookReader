using BookReader.Core.Abstract.Business;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.DTOs.Models
{
    public class BookPage
    {
        public int Number { get; set; }
        public required List<ITextElement> Paragraphs { get; set; }
    }
}
