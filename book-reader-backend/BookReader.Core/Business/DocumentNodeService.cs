using AngleSharp;
using AngleSharp.Dom;
using BookReader.Core.Abstract.Repositories;
using BookReader.Core.Abstract.Services;
using BookReader.Core.DTOs.Models;
using BookReader.Core.Entities;
using BookReader.Core.Enums;
using BookReader.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;


namespace BookReader.Core.Business
{
    public class DocumentNodeService : BaseService<DocumentNodeService>, IDocumentNodeService
    {
        private readonly IStorageService _storageService;
        private readonly IBookRepository _bookRepository;
        private readonly ICacheService _cacheService;
        private const int CHARS_PER_PAGE = 1000;

        public DocumentNodeService(IStorageService storageService,
            ICacheService cacheService,
            IBookRepository bookRepository,
             Microsoft.Extensions.Configuration.IConfiguration config,
            ILogger<DocumentNodeService> logger) : base(config, logger)
        {
           _storageService = storageService;
            _cacheService = cacheService;
            _bookRepository = bookRepository;
        }

        public async Task<ServiceResult<string>> GetRequiredChapterAsync(int bookId, int index, int pageNumber, CancellationToken token)
        {
            var res = string.Empty;
            var cacheKey = CacheExtensions.BuildChacheChapterKey(bookId, index);
            var chapter = await _cacheService.GetAsync<string>(cacheKey);
            if (chapter == null)
            {
                var book = await _bookRepository.GetByIdAsync(bookId, token);
                if (book == null || book.ParsedFilesPath == null)
                    return new ServiceResult<string>(null, $"Cannot find book. Id:{bookId}");
                var fileName = $"{index.ToString()}.json";
                var filePath = Path.Combine(book.ParsedFilesPath, fileName);
                if (string.IsNullOrEmpty(filePath))
                    return new ServiceResult<string>(null, $"Seems the book (Id: {bookId}) haven't been processed. The filepath is empty");

                using var fileStream = await _storageService.GetParsedBookAsync(filePath);
                using var reader = new StreamReader(fileStream);
                chapter = await reader.ReadToEndAsync(token);
                if (string.IsNullOrEmpty(chapter))
                    return new ServiceResult<string>(null, $"Cannot get text from file. Book. Id:{bookId}");

                await _cacheService.SetAsync(cacheKey, chapter);
                if (index > 1)
                {
                    var prevChapterCacheKey = CacheExtensions.BuildChacheChapterKey(bookId, index -1);
                    var prevChapter = await _cacheService.GetAsync<string>(prevChapterCacheKey);
                    if(prevChapter != null)
                        await _cacheService.RemoveAsync(prevChapterCacheKey);
                }
            }

            var chapterObject = JsonSerializer.Deserialize<DocumentNode>(chapter);
            if (chapterObject == null)
                return new ServiceResult<string>(null, $"Cannot deserialize json from file. Book. Id:{bookId}");

            var chapterHtmlResult = await BuildChapterHtmlContent(chapterObject, pageNumber, index);
           
            return new ServiceResult<string>(chapterHtmlResult, null);

        }

        private async Task<string> BuildChapterHtmlContent(DocumentNode chapter,
            int pageNumber,
            int chapterIndex)
        {
            var context = BrowsingContext.New(Configuration.Default);
            var document = await context.OpenNewAsync();

            //var charsOnPage = 0;
            //var needToSkip = pageNumber * CHARS_PER_PAGE - CHARS_PER_PAGE;
           // var filteredParagraps = chapter.Children.Select(p=> new { Count = p.Children.Sum(s=> s.CharsCount), Element = p })
            foreach (var element in chapter.Children)
            {
               CreateHtmlElement(document.Body!, element!, document);
            }
            return document.DocumentElement.OuterHtml;
        }

        private void CreateHtmlElement(IElement parrent, DocumentNode element, IDocument doc)
        {
            var htmlNode = CreateHtmlNodeByTypeSelector[element.NodeType](doc);
            if (!element.Attributes.IsNullOrEmpty())
            {
                foreach (var attr in element.Attributes)
                {
                    htmlNode.SetAttribute(attr.Key, attr.Value);
                }
            }
            
            if (element.NodeType == TextNodeType.Sentence)
            {
                htmlNode.SetAttribute("class", "sentence-text");
                var container = doc.CreateElement("span");
                var sentenceId = element.Attributes.GetValueOrDefault("data-sentence-id");
                if (!string.IsNullOrEmpty(sentenceId))
                {
                    container.SetAttribute("data-sentence-id", sentenceId);
                }               
                container.SetAttribute("class", "sentence");
                container.AppendChild(htmlNode);

                var translateButtonElement = doc.CreateElement("button");
                translateButtonElement.TextContent = "Translate";
                translateButtonElement.SetAttribute("class", "translate-button");
                translateButtonElement.InnerHtml = """
                            <svg xmlns="http://www.w3.org/2000/svg" width="18"
                            height="18"
                            viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                              <circle cx="12" cy="12" r="9"/>
                              <path d="M3 12h18"/>
                              <path d="M12 3a15 15 0 0 1 0 18"/>
                              <path d="M12 3a15 15 0 0 0 0 18"/>
                            </svg>
                            """;
                container.AppendChild(translateButtonElement);
                var newLine = doc.CreateElement("br");
                container.AppendChild(newLine);
                parrent.AppendChild(container);
            }
            else 
            {
                if (element.NodeType == TextNodeType.Word)
                {
                    htmlNode.TextContent = $"{element.Value ?? string.Empty}";
                    parrent.Append(doc.CreateTextNode(" "));
                }    
                    
                parrent.Append(htmlNode);
            }
            if (!element.Children.IsNullOrEmpty())
            {
                foreach (var child in element.Children)
                {
                    CreateHtmlElement(htmlNode, child, doc);
                }
            }
        }

        private Dictionary<TextNodeType, Func<IDocument,IElement>> CreateHtmlNodeByTypeSelector = new()
        {
            { TextNodeType.Paragraph, (doc) => doc.CreateElement("p") },
            { TextNodeType.Chapter, (doc) => doc.CreateElement("h1") },
            { TextNodeType.Image, (doc) => doc.CreateElement("img") },
            { TextNodeType.Sentence, (doc) => doc.CreateElement("span") },
            { TextNodeType.Word, (doc) => doc.CreateElement("span") },
        };

    }
}
