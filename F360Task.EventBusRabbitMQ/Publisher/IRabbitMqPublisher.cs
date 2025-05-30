
namespace F360Task.EventBusRabbitMQ.Publisher
{
    public interface IRabbitMqPublisher
    {
        Task Publish(string exchange, string routingKey, bool mandatory, ReadOnlyMemory<byte> body, CancellationToken cancellationToken);
    }
}