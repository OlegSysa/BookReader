using AngleSharp;
using AngleSharp.Dom;
using BookReader.Core.Abstract.Services;
using BookReader.Core.DTOs.Models;
using BookReader.Core.Enums;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using VersOne.Epub;
namespace BookReader.Core.Business.Parsers
{
    public class EpubToJsonParser //: IParser
    {
        private readonly string _selectors = "p, h1, h2, h3, h4, h5, h6,img, li, blockquote";
        public BookExtension Extension => BookExtension.epub;

        public async Task<JsonInsight> ParseFile(string path)
        {
            var results = new Dictionary<int, string>();
            var book = await EpubReader.ReadBookAsync(path);
            var insightModel = new JsonInsight();
            foreach (var (chapter, index) in book.ReadingOrder.Select((chapter, index) => (chapter, index)))
            {
                var context = BrowsingContext.New(Configuration.Default);
                IDocument document = await context.OpenAsync(req => req.Content(chapter.Content));
                var paragraphs = document.QuerySelectorAll(_selectors);
                //document.Body?.SetAttribute("data-chapter-id", index.ToString());
                for (int i = 0; i < paragraphs.Count; i++)
                {
                    var p = paragraphs[i];
                    var pStyles = p.GetAttribute("style");
                    var parTextObject = new Paragraph()
                    {
                        OrderValueId = $"data-paragraph-id-{i}",
                        CssStyles = pStyles,
                        Selector = p.TagName.ToLower(),
                        ImageSrc = p.GetAttribute("src")
                    };
                  
                    if (string.IsNullOrEmpty(p.TextContent))
                        continue;

                    var sentences = Regex.Matches(p.Text(), @"[^.!?]+(?:[.!?]+|$)")
                        .Select(m => m.Value.Trim())
                        .ToList();
                  
                    for (int j = 0; j < sentences.Count; j++)
                    {
                        var s = sentences[j];
                        var sentenceTextObject = new Sentence()
                        {
                            OrderValueId = $"data-sentence-id-{j}",
                            TextValues = new List<TextValue>()
                        };
                        if (string.IsNullOrEmpty(s))
                            continue;

                        var words = s.Split(' ', ',');
                        for (int k = 0; k < words.Length; k++)
                        {
                           var w = words[k];
                           var textValue = new TextValue()
                           {
                               OrderValueId = $"data-word-id-{k}",
                               Value = w
                           };
                            sentenceTextObject.TextValues.Add(textValue);                           
                        }

                        parTextObject.Sentences.Add(sentenceTextObject);
                    }
                    insightModel.Paragraphs.Add(parTextObject);
                }
            }
            return insightModel;
        }
    }
}
