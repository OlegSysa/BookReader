using MassTransit;

namespace BookReader.BookProcessor.Consumers
{
    public class UploadBookConsumerDefinition : ConsumerDefinition<UploadBookConsumer>
    {
        public UploadBookConsumerDefinition()
        {
            EndpointName = "book-processing";
        }
    }
}
