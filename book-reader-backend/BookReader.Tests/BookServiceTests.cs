using BookReader.Core.Abstract.Repositories;
using BookReader.Core.Abstract.Services;
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

    private BookService _service = null!;

    [SetUp]
    public void Setup()
    {
        _repository = new Mock<IBookRepository>();
        _storage = new Mock<IStorageService>();
        _logger = new Mock<ILogger<BookService>>();
        var configuration = new ConfigurationBuilder().Build();
        _service = new BookService(
            _repository.Object,
            _storage.Object,
            configuration,
            _logger.Object);
    }

    [Test]
    public async Task UploadAsync_WhenFileWasntSavedToStorage_ShouldReturnFailedStatus()
    {
        // Arrange
        using var stream = new MemoryStream();

        // Act
        var result = await _service.UploadAsync(
            stream,
            "book.pdf",
            0,
            1,
            CancellationToken.None);

        // Assert
        Assert.That(result.Status, Is.EqualTo(BookStatus.Failed));
        Assert.That(result.Book, Is.Null);
    }
}