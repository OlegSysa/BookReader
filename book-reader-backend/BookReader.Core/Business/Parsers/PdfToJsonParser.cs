using BookReader.Core.Abstract.Services;
using BookReader.Core.DTOs.Models;
using BookReader.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;

namespace BookReader.Core.Business.Parsers
{
    public class PdfToJsonParser : IParser
    {
        public BookExtension Extension => BookExtension.pdf;
        private readonly IStorageService _storageService;

        public PdfToJsonParser(IStorageService storageService)
        {
            _storageService = storageService;
        }

        public async Task<DocumentNode> ParseFile(string path)
        {
            var chapters = new List<DocumentNode>();

            using var stream = await _storageService.GetBookAsync(path);
            using var document = PdfDocument.Open(stream);
            foreach (var (page, index) in document.GetPages()
                         .Select((page, index) => (page, index)))
            {
                var chapter = new DocumentNode
                {
                    NodeType = TextNodeType.Chapter
                };

                chapter.Attributes.Add(
                    "data-chapter-id",
                    index.ToString());

                chapters.Add(chapter);

                var text = page.Text;

                if (string.IsNullOrWhiteSpace(text))
                    continue;

                var paragraphs = SplitIntoParagraphs(text);

                for (int i = 0; i < paragraphs.Count; i++)
                {
                    var paragraphText = paragraphs[i];

                    var paragraph = new DocumentNode
                    {
                        NodeType = TextNodeType.Paragraph
                    };

                    paragraph.Attributes.Add(
                        "data-paragraph-id",
                        i.ToString());

                    chapter.Children.Add(paragraph);

                    var sentences = Regex.Matches(
                            paragraphText,
                            @"[^.!?]+(?:[.!?]+|$)")
                        .Select(x => x.Value.Trim())
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .ToList();

                    paragraph.CharsCount =
                        sentences.Sum(x => x.Length);

                    for (int j = 0; j < sentences.Count; j++)
                    {
                        var sentenceText = sentences[j];

                        var sentence = new DocumentNode
                        {
                            NodeType = TextNodeType.Sentence,
                            CharsCount = sentenceText.Length
                        };

                        sentence.Attributes.Add(
                            "data-sentence-id",
                            j.ToString());

                        paragraph.Children.Add(sentence);

                        var words = sentenceText
                            .Split(
                                [' ', ','],
                                StringSplitOptions.RemoveEmptyEntries);

                        for (int k = 0; k < words.Length; k++)
                        {
                            var word = new DocumentNode
                            {
                                Value = words[k],
                                NodeType = TextNodeType.Word
                            };

                            word.Attributes.Add(
                                "data-word-id",
                                k.ToString());

                            sentence.Children.Add(word);
                        }
                    }
                }

                chapter.CharsCount =
                    chapter.Children.Sum(x => x.CharsCount);
            }
            var bookNode = new DocumentNode()
            {
                NodeType = TextNodeType.Document,
                CharsCount = chapters.Sum(c => c.CharsCount),
                Children = chapters
            };

            return bookNode;
        }

        private static List<string> SplitIntoParagraphs(string text)
        {
            return Regex
                .Split(text, @"\r?\n\s*\r?\n")
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();
        }
    }
}
