using AngleSharp;
using AngleSharp.Dom;
using BookReader.Core.Abstract.Repositories;
using BookReader.Core.Abstract.Services;
using BookReader.Core.DTOs.Models;
using BookReader.Core.Entities;
using BookReader.Core.Enums;
using BookReader.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
        private const int CHARS_PER_PAGE = 2000;

        public DocumentNodeService(IStorageService storageService,
            ICacheService cacheService,
            IBookRepository bookRepository,
             Microsoft.Extensions.Configuration.IConfiguration config,
            ILogger<DocumentNodeService> logger,
            IHttpContextAccessor httpContextAccessor) : base(config, logger, httpContextAccessor)
        {
            _storageService = storageService;
            _cacheService = cacheService;
            _bookRepository = bookRepository;
        }

        public async Task<ServiceResult<BookViewResult>> GetPageContentAsync(int userId, int bookId,
            int pageNumber,
            CancellationToken token)
        {
            var cacheKey = CacheExtensions.BuildChacheBookKey(userId, bookId);
            var bookState = await _cacheService.GetAsync<BookState>(cacheKey);
            if (bookState == null)
            {
                var book = await _bookRepository.GetByIdAsync(bookId, token);
                if (book == null || book.ParsedFilesPath == null)
                    return new ServiceResult<BookViewResult>(null, $"Cannot find book. Id:{bookId}");
                
                using var fileStream = await _storageService.GetParsedBookAsync(book.ParsedFilesPath);
                using var reader = new StreamReader(fileStream);
                var rawBook = await reader.ReadToEndAsync(token);
                if (string.IsNullOrEmpty(rawBook))
                    return new ServiceResult<BookViewResult>(null, $"Cannot get text from file. Book. Id:{bookId}");

                var bookNode = JsonSerializer.Deserialize<DocumentNode>(rawBook);
                if (bookNode == null)
                    return new ServiceResult<BookViewResult>(null, $"Cannot deserialize json from file. Book. Id:{bookId}");

                var pages = new List<BookPage>();
                //var startPageNumber = 1;
                //if (chapterIndex > 1)
                //{
                //    var prevChapterCacheKey = CacheExtensions.BuildChacheChapterKey(bookId, chapterIndex - 1);
                //    var prevChapter = await _cacheService.GetAsync<BookState>(prevChapterCacheKey);
                //    if (prevChapter != null)
                //    {
                //        //var prevChapterState = JsonSerializer.Deserialize<ChapterState>(prevChapter);

                //            //startPageNumber = prevChapter.NumberOfPages + 1;

                //       //await _cacheService.RemoveAsync(prevChapterCacheKey);
                //    }

                //}
                var paragraphs = bookNode.Children.SelectMany(c => c.Children).ToList();
                foreach (var par in paragraphs)
                {
                    CreatePage(pages, par);
                }
                bookState = new BookState()
                {
                    Pages = pages.ToDictionary(p => p.Number),
                    BookId = bookId,
                    NumberOfPages = pages.Count,
                };
                
            }

            bookState.IsLastPage = bookState.Pages.Count == 0 || bookState.Pages.Count == pageNumber;
            await _cacheService.SetAsync(cacheKey, bookState);

            var bookContent = await BuildChapterHtmlContent(bookState, pageNumber);
            if (string.IsNullOrEmpty(bookContent))
                bookContent = "<div>Empty</div>";
            var res = new BookViewResult()
            {
                Content = bookContent,
                IsLastPage = bookState.IsLastPage,
                NumberOfPages = bookState.NumberOfPages
            };

            return new ServiceResult<BookViewResult>(res, null);
        }

        private async Task<string> BuildChapterHtmlContent(BookState book,
            int pageNumber)
        {
            var context = BrowsingContext.New(Configuration.Default);
            var document = await context.OpenNewAsync();

            if (!book.Pages.TryGetValue(pageNumber, out var currentPage))
                return string.Empty;

            foreach (var par in currentPage.Paragraphs)
            {
                CreateHtmlElement(document.Body!, par, document);
            }
            return document.DocumentElement.OuterHtml;
        }

        private void CreatePage(List<BookPage> pagesList, DocumentNode node)
        {
            var lastPage = pagesList.LastOrDefault();
            if (lastPage == null)
            {
                lastPage = new BookPage()
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
                        var newPage = new BookPage()
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
