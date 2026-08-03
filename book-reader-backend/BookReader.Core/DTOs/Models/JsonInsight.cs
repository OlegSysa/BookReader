namespace BookReader.Core.DTOs.Models
{
    public class JsonInsight 
    {
        public List<Paragraph> Paragraphs { get; set; } = new List<Paragraph>();
    }

    public class Paragraph : BaseTextElement
    {
        public required string Selector { get; set; }
        public string? ImageSrc { get; set; }
        public string? CssStyles { get; set; }
        public List<Sentence> Sentences { get; set; } = new List<Sentence>();
    }
    public class Sentence : BaseTextElement
    {
        public required List<TextValue> TextValues { get; set; }
    }

    public class TextValue : BaseTextElement
    {
        public required string Value { get; set; }
    }

    public class BaseTextElement
    {
        public required string OrderValueId { get; set; }
    }
}
