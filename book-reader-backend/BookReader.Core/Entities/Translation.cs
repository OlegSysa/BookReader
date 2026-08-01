using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.Entities
{
    public class Translation
    {
        public int Id { get; set; }
        public required string SourceLang { get; set; }
        public required string TargetLang { get; set; }
        public required string SourceWord { get; set; }
        public required string TranslatedWord {  get; set; }

    }
}
