using BookReader.BookProcessor.Abstract;
using BookReader.Core.Abstract.Repositories;
using BookReader.Core.Enums;

namespace BookReader.BookProcessor.Services
{
    public class OutboxMessageProcessor : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OutboxMessageProcessor> _logger;

        public OutboxMessageProcessor(
            IServiceScopeFactory scopeFactory,
            
            ILogger<OutboxMessageProcessor> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var handlers = scope.ServiceProvider.GetServices<IOutboxMessageHandler>().ToDictionary(h=> h.Type);
                    var repo = scope.ServiceProvider.GetRequiredService<IOutboxMessageRepository>();
                    var messagesForProcessing = await repo.GetMessagesForProcessingAsync(stoppingToken);
                    var messageGroups = messagesForProcessing.GroupBy(m => m.EventType);
                    foreach (var messageGroup in messageGroups) {
                        if (!handlers.TryGetValue(messageGroup.Key, out var handler))
                                continue;
                        await handler.HandleAsync(messageGroup, stoppingToken);
                    }
                    _logger.LogInformation("Outbox scan completed");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Outbox processing failed");
                }

                await Task.Delay(TimeSpan.FromSeconds(10),stoppingToken);
            }
            }
    }
}
