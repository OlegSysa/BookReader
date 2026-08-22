using BookReader.Core.DTOs.Models;
using BookReader.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.Extensions
{
    public static class DocumentNodeExtensions
    {
        public static int Count(this DocumentNode node)
        {
            if (node.NodeType == TextNodeType.Sentence)
            {
                return node.CharsCount;
            }
            else if (node.NodeType == TextNodeType.Paragraph)
            {
                return node.Children.Sum(s => s.Count());
            }
            else return node.Value?.Length ?? 0;
        }
    }
}
