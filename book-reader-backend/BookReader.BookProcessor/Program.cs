
using AngleSharp;
using BookReader.BookProcessor.Consumers;
using BookReader.Core.Abstract.Repositories;
using BookReader.Core.Abstract.Services;
using BookReader.Core.Business;
using BookReader.Core.Business.Parsers;
using BookReader.Infrastructure.DependencyInjection;
using BookReader.Infrastructure.Persistence;
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
            var builder = Host.CreateApplicationBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("DatabaseConnection");
            builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));
            //builder.Services.ResolveDependencies(builder.Configuration);
            builder.Services.AddScoped<IBookRepository, BookRepository>();
            builder.Services.AddScoped<IChapterRepository, ChapterRepository>();
            builder.Services.AddScoped<IBookParserService, BookParserService>();
            builder.Services.AddScoped<IParser, EpubToHtmlParser>();
            builder.Services.AddMassTransit(x =>
            {
                x.AddConsumer<UploadBookConsumer>();
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });
                    cfg.ConfigureEndpoints(context);
                });
            });

            var host = builder.Build();
            host.Run();
        }
    }
}
