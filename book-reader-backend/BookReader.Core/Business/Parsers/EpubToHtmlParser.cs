using AngleSharp;
using AngleSharp.Dom;
using BookReader.Core.Abstract.Services;
using BookReader.Core.DTOs.Models;
using BookReader.Core.Enums;
using System.Text.RegularExpressions;
using VersOne.Epub;

namespace BookReader.Core.Business.Parsers
{
    public class EpubToHtmlParser //: IParser
    {
        private readonly string _selectors = "p, h1, h2, h3, h4, h5, h6, li, blockquote, img";
        public BookExtension Extension => BookExtension.epub;

        public async Task<Dictionary<int, string>> ParseFile(string path)
        {
            var results = new Dictionary<int, string>();
            var book = await EpubReader.ReadBookAsync(path);
            foreach (var (chapter, index) in book.ReadingOrder.Select((chapter, index) => (chapter, index)))
            {
                var context = BrowsingContext.New(Configuration.Default);
                IDocument document = await context.OpenAsync(req => req.Content(chapter.Content));
                var paragraphs = document.QuerySelectorAll(_selectors);
                document.Body?.SetAttribute("data-chapter-id",index.ToString());
                for (int i = 0; i < paragraphs.Count; i++) 
                { 
                    var p = paragraphs[i];
                    p.SetAttribute("data-paragraph-id", i.ToString());
                    var isImg = p.GetSelector() == "img";
                    if (string.IsNullOrEmpty(p.TextContent) || isImg)
                        continue;

                    var sentences = Regex.Matches(p.Text(), @"[^.!?]+(?:[.!?]+|$)")
                        .Select(m => m.Value.Trim())
                        .ToList();
                    p.InnerHtml = string.Empty;
                    for (int j = 0; j < sentences.Count; j++)
                    {
                        var s = sentences[j];
                        if (string.IsNullOrEmpty(s))
                            continue;
                        var container = document.CreateElement("div");
                        container.SetAttribute("data-sentence-id", j.ToString());
                        container.SetAttribute("class", "sentence");

                        var sentenceSpan = document.CreateElement("span");
                        sentenceSpan.SetAttribute("class", "sentence-text");
                        container.Append(sentenceSpan);

                        var words = s.Split(' ', ',');
                        for (int k = 0; k < words.Length; k++) 
                        { 
                            var w = words[k];
                            var wordSpan = document.CreateElement("span");
                            wordSpan.TextContent = w;
                            wordSpan.SetAttribute("data-word-id", k.ToString());
                            sentenceSpan.Append(wordSpan);
                            var isLastWordInSentence = k == words.Length - 1;
                            sentenceSpan.Append(document.CreateTextNode(" "));
                        }

                        var translateButtonElement = document.CreateElement("button");
                        translateButtonElement.TextContent = "Translate";
                        translateButtonElement.SetAttribute("class", "translate-button");
                        translateButtonElement.InnerHtml = """
                            <svg xmlns="http://www.w3.org/2000/svg"
                             width="18"
                             height="18"
                             viewBox="0 0 24 24"
                             fill="none"
                             stroke="currentColor"
                             stroke-width="2"
                             stroke-linecap="round"
                             stroke-linejoin="round">
                            <path d="M5 8h14"/>
                            <path d="M5 12h6"/>
                            <path d="M13 20l4-10 4 10"/>
                            <path d="M14.5 16h5"/>
                            </svg>
                            """;
                        container.Append(translateButtonElement);
                        var newLineElement = document.CreateElement("br");
                        container.Append(newLineElement);
                        p.Append(container);
                    }
                }
               results.Add(index, document.DocumentElement.OuterHtml);
            }
           return results;
        }
    }
}
