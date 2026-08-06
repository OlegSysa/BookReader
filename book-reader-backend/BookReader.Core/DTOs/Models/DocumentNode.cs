using BookReader.Core.Enums;
using System.Text.Json.Serialization;

namespace BookReader.Core.DTOs.Models
{
    
    public class DocumentNode 
    {
        public List<DocumentNode> Children { get; set; } = new List<DocumentNode>();
        public required TextNodeType NodeType { get; set; }
        public string? Value { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
    }

    
}
