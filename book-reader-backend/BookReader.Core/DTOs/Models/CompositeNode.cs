using BookReader.Core.Abstract.Business;
using BookReader.Core.Enums;
using System.Text.Json.Serialization;

namespace BookReader.Core.DTOs.Models
{
    public class CompositeNode : ITextElement
    {
        public List<ITextElement> Children { get; set; } = new();
        public required TextNodeType NodeType { get; init; }
        public Dictionary<string, string> Attributes { get; set; } = new ();
        public int CharsCount => Children.Sum(c => c.CharsCount);

        public string GetText()
        {
            return string.Concat(Children.Select(c => c.GetText()));
        }
    }

    public class TextNode : ITextElement
    {
        public required TextNodeType NodeType { get; init; }
        public required string Value { get; init; }
        public Dictionary<string, string> Attributes { get; set; } = new ();
        public int CharsCount => Value.Length;
        public string GetText() => Value;
    }
}
