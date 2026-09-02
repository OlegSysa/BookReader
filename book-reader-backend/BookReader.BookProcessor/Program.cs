using Azure.Identity;
using BookReader.BookProcessor.Abstract;
using BookReader.BookProcessor.Consumers;
using BookReader.BookProcessor.Services;
using BookReader.BookProcessor.Services.Handlers;
using BookReader.Core.Abstract.Events;
using BookReader.Core.Abstract.Repositories;
using BookReader.Core.Abstract.Services;
using BookReader.Core.Business;
using BookReader.Core.Business.Parsers;
using BookReader.Infrastructure.Persistence;
using BookReader.Infrastructure.Persistence.Configurations;
using BookReader.Infrastructure.Repositories;
using BookReader.Infrastructure.Services;
using BookReader.Infrastructure.Services.Messaging;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace BookReader.BookProcessor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddHttpContextAccessor();
            if (!builder.Environment.IsDevelopment())
            {
                var keyVaultUrl = new Uri(
                    builder.Configuration["KeyVault:Url"]!);

                builder.Configuration.AddAzureKeyVault(
                    keyVaultUrl,
                    new DefaultAzureCredential());
            }
            builder.Host.UseSerilog((context, services, configuration) =>
            {
                configuration.ReadFrom.Configuration(context.Configuration);

                if (!context.HostingEnvironment.IsDevelopment())
                {
                    configuration.WriteTo.ApplicationInsights(
                        context.Configuration["ApplicationInsights:ConnectionString"],
                        TelemetryConverter.Traces);
                }
            });
            var connectionString = builder.Configuration.GetConnectionString("DatabaseConnection");
            builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));
            builder.Services.AddScoped<IStorageService, AzureStorageService>();
            builder.Services.AddScoped<IBookRepository, BookRepository>();
            builder.Services.AddScoped<IOutboxMessageRepository, OutboxMessageRepository>();
            builder.Services.AddScoped<IEventPublisher, MassTransitEventPublisher>();
            builder.Services.AddScoped<IBookParserService, BookParserService>();
            builder.Services.AddScoped<IParser, EpubToJsonParser>();
            builder.Services.AddScoped<IParser, Fb2ToJsonParser>();
            builder.Services.AddScoped<IOutboxMessageHandler, BookDeletedHandler>();
            builder.Services.Configure<AzureStorageOptions>(builder.Configuration.GetSection("AzureStorage"));
            builder.Services.AddHostedService<OutboxMessageProcessor>();
            if (builder.Configuration.GetValue<bool>("Messaging:Enabled"))
            {
                builder.Services.AddMassTransit(x =>
                {
                   x.AddConsumer<UploadBookConsumer, UploadBookConsumerDefinition>();

                    if (builder.Configuration.GetValue<bool>("Messaging:ServiceBusEnabled"))
                    {
                        x.UsingAzureServiceBus((context, cfg) =>
                        {
                            cfg.Host(builder.Configuration["ServiceBus:ConnectionString"]);

                            cfg.ReceiveEndpoint("book-processing", e =>
                            {
                                e.ConfigureConsumeTopology = false;
                                e.ConfigureConsumer<UploadBookConsumer>(context);
                            });
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

                            cfg.ConfigureEndpoints(context);
                        });
                    }
                });
            }
            var app = builder.Build();
            app.MapGet("/health", () => Results.Ok("BookProcessor is running"));
            app.Run();
        }
    }
}
