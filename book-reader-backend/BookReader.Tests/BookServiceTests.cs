using BookReader.Core.Abstract.Events;
using BookReader.Core.Abstract.Repositories;
using BookReader.Core.Abstract.Services;
using BookReader.Core.DTOs.Models;
using BookReader.Core.Enums;
using BookReader.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;


[TestFixture]
public class BookServiceTests
{
    private Mock<IBookRepository> _repository = null!;
    private Mock<IStorageService> _storage = null!;
    private Mock<ILogger<BookService>> _logger = null!;
    private Mock<IEventPublisher> _publisher = null!;
    private Mock<IOutboxMessageRepository> _outboxMessageRepo = null!;

    private BookService _service = null!;

    [SetUp]
    public void Setup()
    {
        _repository = new Mock<IBookRepository>();
        _storage = new Mock<IStorageService>();
        _logger = new Mock<ILogger<BookService>>();
        _publisher = new Mock<IEventPublisher>();
        _outboxMessageRepo = new Mock<IOutboxMessageRepository>();
        var configuration = new ConfigurationBuilder().Build();
        _service = new BookService(
            _repository.Object,
            _storage.Object,
            _publisher.Object,
            configuration,
            _outboxMessageRepo.Object,
            _logger.Object);
    }

    [Test]
    public async Task UploadAsync_WhenFileWasntSavedToStorage_ShouldReturnFailedStatus()
    {
        // Arrange
        using var stream = new MemoryStream();
        var details = new UploadBookDetails("book.pdf", "TestTitle", "TestAuthor", 0, 0);
        // Act
        var result = await _service.UploadAsync(
            stream,
            details,
            CancellationToken.None);

        // Assert
        Assert.That(result.Data?.Status, Is.EqualTo(BookStatus.Failed));
        Assert.That(result.Data?.Book, Is.Null);
    }
}