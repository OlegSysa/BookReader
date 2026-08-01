using BookReader.Infrastructure.DependencyInjection;
using Serilog;
using StackExchange.Redis;
namespace BookReader.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();

            builder.Host.UseSerilog();
            builder.Services.Resolve(builder.Configuration);
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("front", policy =>
                {
                    policy
                        .WithOrigins("http://localhost:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration =
                    builder.Configuration["ConnectionStrings:RedisConnection"];
            });
            builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
            {
                return ConnectionMultiplexer.Connect(
                    builder.Configuration["ConnectionStrings:RedisConnection"]!);
            });
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            try
            {
                var app = builder.Build();
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();
                app.UseCors("front");

                app.MapControllers();

                app.Run();
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
