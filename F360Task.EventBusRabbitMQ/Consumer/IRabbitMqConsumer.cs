namespace F360Task.EventBusRabbitMQ.Consumer;

public interface IRabbitMqConsumer
{
    Task Consumer(string queueName, string consumerTag, CancellationToken cancellationToken);
}