using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.DTOs.Models
{
    public class LearningWordModel
    {
        public int Id { get; set; }
        public int TranslationId { get; set; }
        public int UserId { get; set; }
        public required string SourceValue { get; set; }
        public required string Translation{ get; set; }
    }
}
