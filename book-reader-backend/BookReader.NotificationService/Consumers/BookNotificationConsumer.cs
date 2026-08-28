using BookReader.Core.Events;
using BookReader.NotificationService.Abstract;
using MassTransit;

namespace BookReader.NotificationService.Consumers
{
    public class BookNotificationConsumer : IConsumer<BookNotificationEvent>
    {
        private readonly INotificationManager _notificationManager;
        public BookNotificationConsumer(
    INotificationManager notificationManager)
        {
            _notificationManager = notificationManager;
        }
        public async Task Consume(ConsumeContext<BookNotificationEvent> context)
        {
            using (Serilog.Context.LogContext.PushProperty("CorrelationId", context.CorrelationId))
            {
                await _notificationManager.SendAsync(context.Message.UserId, context.Message);
            }
        }

    }
}
