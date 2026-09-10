using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.Entities
{
    public class LearningWord
    {
        public int Id { get; set; }
        public int TranslationId { get; set; }
        public int UserId { get; set; }

        public Translation Translation { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
