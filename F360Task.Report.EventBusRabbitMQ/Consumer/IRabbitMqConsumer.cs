namespace F360Task.Report.EventBusRabbitMQ.Consumer;

public interface IRabbitMqConsumer
{
    Task Consumer(
        string exchange,
        string queueName,
        string consumerTag,
        CancellationToken cancellationToken);
}