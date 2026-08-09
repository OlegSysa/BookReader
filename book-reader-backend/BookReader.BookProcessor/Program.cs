using AngleSharp;
using BookReader.BookProcessor.Consumers;
using BookReader.Core.Abstract.Repositories;
using BookReader.Core.Abstract.Services;
using BookReader.Core.Business;
using BookReader.Core.Business.Parsers;
using Microsoft.AspNetCore.Builder;
using BookReader.Infrastructure.Persistence;
using BookReader.Infrastructure.Persistence.Configurations;
using BookReader.Infrastructure.Repositories;
using BookReader.Infrastructure.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace BookReader.BookProcessor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("DatabaseConnection");
            builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));
            builder.Services.AddScoped<IStorageService, AzureStorageService>();
            builder.Services.AddScoped<IBookRepository, BookRepository>();
            builder.Services.AddScoped<IBookParserService, BookParserService>();
            builder.Services.AddScoped<IParser, EpubToJsonParser>();
            builder.Services.Configure<AzureStorageOptions>(builder.Configuration.GetSection("AzureStorage"));
            if (builder.Configuration.GetValue<bool>("Messaging:Enabled"))
            {
                builder.Services.AddMassTransit(x =>
                {
                    x.AddConsumer<UploadBookConsumer>();

                    if (builder.Configuration.GetValue<bool>("Messaging:ServiceBusEnabled"))
                    {
                        x.UsingAzureServiceBus((context, cfg) =>
                        {
                            cfg.Host(builder.Configuration["ServiceBus:ConnectionString"]);

                            cfg.ReceiveEndpoint("book-processing", e =>
                            {
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
