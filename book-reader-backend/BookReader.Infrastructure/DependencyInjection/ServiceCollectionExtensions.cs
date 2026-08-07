using BookReader.Core.Abstract.Events;
using BookReader.Core.Abstract.Repositories;
using BookReader.Core.Abstract.Services;
using BookReader.Core.Business;
using BookReader.Core.Business.Parsers;
using BookReader.Core.Events;
using BookReader.Core.Services;
using BookReader.Infrastructure.Persistence;
using BookReader.Infrastructure.Persistence.Configurations;
using BookReader.Infrastructure.Repositories;
using BookReader.Infrastructure.Services;
using BookReader.Infrastructure.Services.Messaging;
using BookReader.Infrastructure.Services.Messaging.Handlers;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace BookReader.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ResolveDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<ITranslationRespository, TranslationRepository>();
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IStorageService, AzureStorageService>();
        services.AddScoped<IDocumentNodeService, DocumentNodeService>();
        services.AddScoped<IEventPublisher, RabbitMqEventPublisher>();
        services.AddScoped<IBookParserService, BookParserService>();
        services.AddScoped<ITranslationService, TranslationService>();
        services.AddScoped<ICacheService, RedisService>();
        //services.AddScoped<IEventHandler<BookUploadedEvent>, BookUploadedEventHandler>();
        if (configuration.GetValue<bool>("Cache:WarmupEnabled"))
        {
            services.AddHostedService<WarmupService>();
        }

        return services;
    }

    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DatabaseConnection");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration["ConnectionStrings:RedisConnection"];
        });

        services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            return ConnectionMultiplexer.Connect(
                configuration["ConnectionStrings:RedisConnection"]!);
        });

        services.Configure<AzureStorageOptions>(configuration.GetSection("AzureStorage"));

        return services;
    }


}