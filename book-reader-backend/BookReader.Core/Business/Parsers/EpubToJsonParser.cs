using AngleSharp;
using AngleSharp.Css.Dom;
using AngleSharp.Dom;
using BookReader.Core.Abstract.Services;
using BookReader.Core.DTOs.Models;
using BookReader.Core.Enums;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using VersOne.Epub;
namespace BookReader.Core.Business.Parsers
{
    public class EpubToJsonParser : IParser
    {
        private readonly string _selectors = "p, h1, h2, h3, h4, h5, h6,img, li, blockquote";
        private readonly IStorageService _storageService;
        public BookExtension Extension => BookExtension.epub;
        public EpubToJsonParser(IStorageService storageService)
        {
            _storageService = storageService;
        }
        public async Task<IEnumerable<DocumentNode>> ParseFile(string path)
        {

            var chaptersResult = new List<DocumentNode>();
            using var stream = await _storageService.GetBookAsync(path);
            var book = await EpubReader.ReadBookAsync(stream);
            foreach (var (chapter, index) in book.ReadingOrder.Select((chapter, index) => (chapter, index)))
            {
                var context = BrowsingContext.New(Configuration.Default);
                IDocument document = await context.OpenAsync(req => req.Content(chapter.Content));
                
                var chapterElement = new DocumentNode()
                {
                    NodeType = TextNodeType.Chapter
                };

                chapterElement.Attributes.Add("data-chapter-id", index.ToString());
                chaptersResult.Add(chapterElement);

                var paragraphs = document.QuerySelectorAll(_selectors);
                for (int i = 0; i < paragraphs.Count; i++)
                {
                    var p = paragraphs[i];
                    var pStyles = p.GetAttribute("style");
                    var paragraph = new DocumentNode()
                    {
                        NodeType = TextNodeType.Paragraph
                    };
                    chapterElement.Children.Add(paragraph);
                    paragraph.Attributes.Add("data-paragraph-id", i.ToString());
                    if (pStyles != null)
                    {
                        paragraph.Attributes.Add("style", pStyles);
                    }
                    
                    if (string.IsNullOrEmpty(p.TextContent))
                        continue;

                    var sentences = Regex.Matches(p.Text(), @"[^.!?]+(?:[.!?]+|$)")
                        .Select(m => m.Value.Trim())
                        .ToList();
                  
                    for (int j = 0; j < sentences.Count; j++)
                    {
                        var s = sentences[j];
                        if (string.IsNullOrEmpty(s))
                            continue;
                        var sentenceTextObject = new DocumentNode()
                        {
                            NodeType = TextNodeType.Sentence
                        };
                        sentenceTextObject.Attributes.Add("data-sentence-id", j.ToString());
                        paragraph.Children.Add(sentenceTextObject);
                        var words = s.Split(' ', ',');
                        for (int k = 0; k < words.Length; k++)
                        {
                           var w = words[k];
                           var textValue = new DocumentNode()
                           {
                              Value = w,
                              NodeType = TextNodeType.Word
                           };
                            textValue.Attributes.Add("data-word-id", k.ToString());
                            sentenceTextObject.Children.Add(textValue);                           
                        }
                    }
                }
            }
            return chaptersResult;
        }
    }
}
