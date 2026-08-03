using BookReader.Core.Abstract.Events;
using BookReader.Core.Abstract.Repositories;
using BookReader.Core.Abstract.Services;
using BookReader.Core.Business;
using BookReader.Core.Business.Parsers;
using BookReader.Core.Events;
using BookReader.Core.Services;
using BookReader.Infrastructure.Persistence;
using BookReader.Infrastructure.Repositories;
using BookReader.Infrastructure.Services;
using BookReader.Infrastructure.Services.Messaging;
using BookReader.Infrastructure.Services.Messaging.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookReader.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ResolveDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DatabaseConnection");
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IChapterRepository, ChapterRepository>();
        services.AddScoped<ITranslationRespository, TranslationRepository>();
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IStorageService, LocalStorageService>();
        services.AddScoped<IChapterService, ChapterService>();
        services.AddScoped<IEventPublisher, RabbitMqEventPublisher>();
        services.AddScoped<IBookParserService, BookParserService>();
        services.AddScoped<ITranslationService, TranslationService>();
        services.AddScoped<ICacheService, RedisService>();
        services.AddScoped<IEventHandler<BookUploadedEvent>, BookUploadedEventHandler>();
        services.AddScoped<IParser, EpubToHtmlParser>();
        services.AddHostedService<WarmupService>();

        return services;
    }


}