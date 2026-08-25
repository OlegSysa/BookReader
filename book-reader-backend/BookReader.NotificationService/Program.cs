using BookReader.Infrastructure.Persistence;
using BookReader.NotificationService.Abstract;
using BookReader.NotificationService.Consumers;
using BookReader.NotificationService.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace BookReader.NotificationService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton<INotificationManager, NotificationManager>();
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    var jwt = builder.Configuration.GetSection("Jwt");

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwt["Issuer"],
                        ValidAudience = jwt["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwt["Key"]!))
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            context.Token = context.Request.Cookies["access_token"];
                            return Task.CompletedTask;
                        }
                    };
                });

            builder.Services.AddAuthorization();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("front", policy =>
                {
                    policy.WithOrigins(
                       "http://localhost:5173",
                       "https://green-island-05796e403.7.azurestaticapps.net",
                       "https://www.bookly.world",
                       "https://bookly.world")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
                });
            });
            if (builder.Configuration.GetValue<bool>("Messaging:Enabled"))
            {
                builder.Services.AddMassTransit(x =>
                {
                    x.AddConsumer<BookNotificationConsumer>();

                    if (builder.Configuration.GetValue<bool>("Messaging:ServiceBusEnabled"))
                    {
                        x.UsingAzureServiceBus((context, cfg) =>
                        {
                            cfg.Host(builder.Configuration["ServiceBus:ConnectionString"]);
                            //cfg.ConfigureEndpoints(context);
                            cfg.ReceiveEndpoint("book-notifications", e =>
                            {
                                e.ConfigureConsumeTopology = false;
                                e.ConfigureConsumer<BookNotificationConsumer>(context);
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

                            cfg.ReceiveEndpoint("book-notifications", e =>
                            {
                                e.ConfigureConsumeTopology = false;
                                e.ConfigureConsumer<BookNotificationConsumer>(context);
                            });
                        });
                    }
                });
            }
            var app = builder.Build();
            app.UseCors("front");
            app.UseAuthorization();
            app.MapGet("/health", () => Results.Ok("BookNotificationsService is running"));
            app.MapGet("/api/notifications/stream", async (HttpContext context, INotificationManager connectionManager,  ILogger<Program> logger) =>
            {
                var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim == null)
                {
                    return Results.Unauthorized();
                }

                var userId = int.Parse(userIdClaim.Value);
                context.Response.ContentType = "text/event-stream";
                context.Response.Headers.CacheControl = "no-cache";
                connectionManager.Add(userId, context.Response);
                try
                {
                    await context.Response.Body.FlushAsync();

                    await Task.Delay(
                        Timeout.Infinite,
                        context.RequestAborted);
                }
                catch (OperationCanceledException)
                {
                }
                finally
                {
                    logger.LogInformation(
       "SSE disconnected. UserId: {UserId}, Aborted: {Aborted}",
       userId,
       context.RequestAborted.IsCancellationRequested);
                    connectionManager.Remove(userId, context.Response);
                }

                return Results.Empty;
            }).RequireAuthorization();
            app.Run();
        }
    }
}
