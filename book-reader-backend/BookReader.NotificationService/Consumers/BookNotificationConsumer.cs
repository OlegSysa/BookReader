using BookReader.Core.Events;
using BookReader.NotificationService.Abstract;
using BookReader.NotificationService.Services;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;

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
        public async Task Consume(ConsumeContext<BookNotificationEvent> context) =>
            await _notificationManager.SendAsync(context.Message.UserId, context.Message);
    }
}
