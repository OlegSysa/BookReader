using BookReader.Infrastructure.DependencyInjection;
using BookReader.Infrastructure.Services.Messaging.Handlers;
using MassTransit;
using Serilog;
using StackExchange.Redis;
namespace BookReader.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseSerilog();
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
            
            builder.Services
                .ResolveDependencies(builder.Configuration)
                .AddInfrastructureServices(builder.Configuration);

            if (builder.Configuration.GetValue<bool>("Messaging:MassTransitEnabled"))
            {
                builder.Services.AddMassTransit(x => 
                {
                    x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.Host("localhost", "/", h =>
                                {
                                    h.Username("guest");
                                    h.Password("guest");
                                });
                        });
                });
            }

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
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
