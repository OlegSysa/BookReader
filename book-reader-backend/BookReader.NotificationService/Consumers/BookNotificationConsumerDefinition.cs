using MassTransit;

namespace BookReader.NotificationService.Consumers
{
    public class BookNotificationConsumerDefinition : ConsumerDefinition<BookNotificationConsumer>
    {
        public BookNotificationConsumerDefinition()
        {
            EndpointName = "book-notifications";
        }
    }
}
