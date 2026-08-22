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
using System.Text.Json;


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

        public async Task<ServiceResult<ChapterViewResult>> GetRequiredChapterAsync(int bookId,
            int chapterIndex,
            int pageNumber, 
            CancellationToken token)
        {
            var cacheKey = CacheExtensions.BuildChacheChapterKey(bookId, chapterIndex);
            var chapter = await _cacheService.GetAsync<ChapterState>(cacheKey);
            if (chapter == null)
            {
                var book = await _bookRepository.GetByIdAsync(bookId, token);
                if (book == null || book.ParsedFilesPath == null)
                    return new ServiceResult<ChapterViewResult>(null, $"Cannot find book. Id:{bookId}");
                var fileName = $"{chapterIndex.ToString()}.json";
                var filePath = Path.Combine(book.ParsedFilesPath, fileName);
                if (string.IsNullOrEmpty(filePath))
                    return new ServiceResult<ChapterViewResult>(null, $"Seems the book (Id: {bookId}) haven't been processed. The filepath is empty");

                using var fileStream = await _storageService.GetParsedBookAsync(filePath);
                using var reader = new StreamReader(fileStream);
                var rawChapter = await reader.ReadToEndAsync(token);
                if (string.IsNullOrEmpty(rawChapter))
                    return new ServiceResult<ChapterViewResult>(null, $"Cannot get text from file. Book. Id:{bookId}");

                var chapterNode = JsonSerializer.Deserialize<DocumentNode>(rawChapter!);
                if (chapterNode == null)
                    return new ServiceResult<ChapterViewResult>(null, $"Cannot deserialize json from file. Book. Id:{bookId}");

                var chapterPages = new List<Page>();
                foreach (var par in chapterNode.Children)
                {
                    CreatePage(chapterPages, par);
                }
                chapter = new ChapterState()
                { 
                    Index = chapterIndex,
                    Pages = chapterPages.ToDictionary(p=> p.Number),
                    BookId = bookId,
                    IsLastChapter = book.ChaptersCount == chapterIndex,
                    IsLastPage = chapterPages.Count == pageNumber,
                    NumberOfPages = chapterPages.Count
                };
                await _cacheService.SetAsync(cacheKey, chapter);
                if (chapterIndex > 1)
                {
                    var prevChapterCacheKey = CacheExtensions.BuildChacheChapterKey(bookId, chapterIndex - 1);
                    var prevChapter = await _cacheService.GetAsync<string>(prevChapterCacheKey);
                    if (prevChapter != null)
                        await _cacheService.RemoveAsync(prevChapterCacheKey);
                }
            }

            var chapterContent = await BuildChapterHtmlContent(chapter, pageNumber, chapterIndex);
            if (string.IsNullOrEmpty(chapterContent))
                chapterContent = "<div>Empty</div>";
            var res = new ChapterViewResult() { 
                Content  = chapterContent,
                Index = chapter.Index,
                IsLastChapter = chapter.IsLastChapter,
                IsLastPage = chapter.IsLastPage,
                NumberOfPages = chapter.NumberOfPages
            };

            return new ServiceResult<ChapterViewResult>(res, null);
        }

        private async Task<string> BuildChapterHtmlContent(ChapterState chapter,
            int pageNumber,
            int chapterIndex)
        {
            var context = BrowsingContext.New(Configuration.Default);
            var document = await context.OpenNewAsync();

            if (!chapter.Pages.TryGetValue(pageNumber, out var currentPage))
                return string.Empty;

            foreach (var element in currentPage!.Paragraphs)
            {
                CreateHtmlElement(document.Body!, element!, document);
            }
            return document.DocumentElement.OuterHtml;
        }

        private void CreatePage(List<Page> pagesList, DocumentNode node)
        {
            var lastPage = pagesList.LastOrDefault();
            if (lastPage == null)
            {
                lastPage = new Page()
                {
                    Number = 1,
                    Paragraphs = new List<DocumentNode>() { node }
                };
                pagesList.Add(lastPage);
            }
            else
            {
                var lastPageCount = lastPage.Paragraphs.Sum(p => p.Count());
                var totalCharsCount = lastPageCount + node.Count();
                if (totalCharsCount >= CHARS_PER_PAGE)
                {
                    var newPagesCount = (int)Math.Ceiling((double)node.CharsCount / CHARS_PER_PAGE);
                    var nodeParagraphs = node.Children;
                    var skippedParagraphs = 0;
                    var lastAddedPageNumber = lastPage.Number;
                    for (int i = 0; i < newPagesCount; i++)
                    {
                        var pageCharsCount = 0;
                        var newPageParagraphs = nodeParagraphs.Skip(skippedParagraphs).TakeWhile(p =>
                        {
                            pageCharsCount += p.CharsCount;
                            return pageCharsCount <= CHARS_PER_PAGE;
                        }).ToList();
                        skippedParagraphs += newPageParagraphs.Count;
                        lastAddedPageNumber++;
                        var newPage = new Page()
                        {
                            Paragraphs = newPageParagraphs,
                            Number = lastAddedPageNumber
                        };
                        pagesList.Add(newPage);
                    }
                }
                else
                {
                    lastPage.Paragraphs.Add(node);
                }
            }
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

        private Dictionary<TextNodeType, Func<IDocument, IElement>> CreateHtmlNodeByTypeSelector = new()
        {
            { TextNodeType.Paragraph, (doc) => doc.CreateElement("p") },
            { TextNodeType.Chapter, (doc) => doc.CreateElement("h1") },
            { TextNodeType.Image, (doc) => doc.CreateElement("img") },
            { TextNodeType.Sentence, (doc) => doc.CreateElement("span") },
            { TextNodeType.Word, (doc) => doc.CreateElement("span") },
        };

    }
}
