using AngleSharp;
using AngleSharp.Dom;
using BookReader.Core.Abstract.Services;
using BookReader.Core.Enums;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using VersOne.Epub;

namespace BookReader.Core.Business.Parsers
{
    public class EpubParser : IParser
    {
        private readonly string _selectors = "p, h1, h2, h3, h4, h5, h6, li, blockquote";
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
                    if (string.IsNullOrEmpty(p.TextContent))
                        continue;
                    
                    var sentences = p.Text().Split(['.',';',':']);
                    p.InnerHtml = string.Empty;
                    for (int j = 0; j < sentences.Length; j++)
                    {
                        var s = sentences[j];
                        if (string.IsNullOrEmpty(s))
                            continue;
                        var sentenceSpan = document.CreateElement("span");
                        sentenceSpan.SetAttribute("data-sentence-id", j.ToString());
                       
                        var words = s.Split(' ', ',');
                        for (int k = 0; k < words.Length; k++) 
                        { 
                            var w = words[k];
                            if (k + 1 == words.Length)
                                w += ". ";
                                var wordSpan = document.CreateElement("span");
                            wordSpan.TextContent = w + ' ';
                            wordSpan.SetAttribute("data-word-id", k.ToString());

                            sentenceSpan.Append(wordSpan);
                            
                              
                        }

                        p.AppendChild(sentenceSpan);
                    }
                }
               results.Add(index, document.DocumentElement.OuterHtml);
            }
           return results;
        }
    }
}
