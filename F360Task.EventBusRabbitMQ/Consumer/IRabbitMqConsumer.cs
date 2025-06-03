namespace F360Task.EventBusRabbitMQ.Consumer;

public interface IRabbitMqConsumer
{
    Task Consumer(
        string exchange,
        string queueName,
        string consumerTag,
        CancellationToken cancellationToken);
}