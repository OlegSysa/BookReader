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
        services.AddScoped<IEventPublisher, MassTransitEventPublisher>();
        services.AddScoped<IBookParserService, BookParserService>();
        services.AddScoped<ITranslationService, TranslationService>();
        

        return services;
    }

    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DatabaseConnection");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));
       
        services.ConfigureCaching(configuration);
        services.Configure<AzureStorageOptions>(configuration.GetSection("AzureStorage"));

        return services;
    }

    public static IServiceCollection ConfigureCaching(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (configuration["Cache:Provider"] == "Redis")
        {
            services.AddScoped<ICacheService, RedisService>();
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration["ConnectionStrings:RedisConnection"];
            });

            services.AddSingleton<IConnectionMultiplexer>(_ =>
            {
                return ConnectionMultiplexer.Connect(
                    configuration["ConnectionStrings:RedisConnection"]!);
            });
        }
        else
        {
            services.AddMemoryCache();
            services.AddScoped<ICacheService, MemoryCacheService>();
        }
        if (configuration.GetValue<bool>("Cache:WarmupEnabled"))
        {
            services.AddHostedService<WarmupService>();
        }

        return services;
    }

    public static IServiceCollection ConfigureMessaging(
    this IServiceCollection services,
    IConfiguration configuration)
    {
        if (configuration.GetValue<bool>("Messaging:Enabled"))
        {
            services.AddMassTransit(x =>
            {
                if (configuration.GetValue<bool>("Messaging:ServiceBusEnabled"))
                {
                    x.UsingAzureServiceBus((context, cfg) =>
                    {
                        cfg.Host(configuration["ServiceBus:ConnectionString"]);
                    });
                }
                else
                {
                    x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host("localhost", "/", h =>
                        {
                            h.Username("guest");
                            h.Password("guest");
                        });
                    });
                }
            });
        }

        return services;
    }


}