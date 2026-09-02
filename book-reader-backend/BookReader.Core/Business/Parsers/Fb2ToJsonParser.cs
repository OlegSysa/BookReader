using BookReader.Core.Abstract.Services;
using BookReader.Core.DTOs.Models;
using BookReader.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace BookReader.Core.Business.Parsers
{
    public class Fb2ToJsonParser : IParser
    {
        public BookExtension Extension => BookExtension.fb2;
        private readonly IStorageService _storageService;
        public Fb2ToJsonParser(IStorageService storageService)
        {
            _storageService = storageService;
        }
        public async Task<DocumentNode> ParseFile(string path)
        {
            using var stream = await _storageService.GetBookAsync(path);
            var document = await XDocument.LoadAsync(
               stream,
               LoadOptions.None,
               CancellationToken.None);

            XNamespace ns = document.Root!.Name.Namespace;
            var body = document.Root.Element(ns + "body");
            if (body == null)
            {
                throw new InvalidOperationException(
                    "FB2 document does not contain body element.");
            }
            var sections = body.Elements(ns + "section").ToList();
            var chapters = new List<DocumentNode>();
            for (int i = 0; i < sections.Count; i++)
            {
                var section = sections[i];

                var chapter = new DocumentNode
                {
                    NodeType = TextNodeType.Chapter
                };

                chapter.Attributes.Add(
                    "data-chapter-id",
                    i.ToString());

                chapters.Add(chapter);

                var paragraphs = section
                    .Elements(ns + "p")
                    .ToList();

                for (int j = 0; j < paragraphs.Count; j++)
                {
                    var paragraphElement = paragraphs[j];

                    var paragraph = new DocumentNode
                    {
                        NodeType = TextNodeType.Paragraph
                    };

                    paragraph.Attributes.Add(
                        "data-paragraph-id",
                        j.ToString());

                    chapter.Children.Add(paragraph);

                    var text = paragraphElement.Value.Trim();

                    if (string.IsNullOrWhiteSpace(text))
                        continue;

                    var sentences = Regex
                        .Matches(text, @"[^.!?]+(?:[.!?]+|$)")
                        .Select(m => m.Value.Trim())
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .ToList();

                    paragraph.CharsCount = sentences.Sum(s => s.Length);

                    for (int k = 0; k < sentences.Count; k++)
                    {
                        var sentenceText = sentences[k];

                        var sentence = new DocumentNode
                        {
                            NodeType = TextNodeType.Sentence,
                            CharsCount = sentenceText.Length
                        };

                        sentence.Attributes.Add(
                            "data-sentence-id",
                            k.ToString());

                        paragraph.Children.Add(sentence);

                        var words = sentenceText.Split(
                            new[] { ' ', ',' },
                            StringSplitOptions.RemoveEmptyEntries);

                        for (int l = 0; l < words.Length; l++)
                        {
                            var word = words[l];

                            var wordNode = new DocumentNode
                            {
                                Value = word,
                                NodeType = TextNodeType.Word
                            };

                            wordNode.Attributes.Add(
                                "data-word-id",
                                l.ToString());

                            sentence.Children.Add(wordNode);
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
    }
}
