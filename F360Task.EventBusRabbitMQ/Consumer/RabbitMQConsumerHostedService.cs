namespace F360Task.EventBusRabbitMQ.Consumer;

public class RabbitMQConsumerHostedService : IHostedService
{

    private readonly IRabbitMqConsumer _rabbitMqConsumer;
    private readonly ILogger<RabbitMQConsumerHostedService> _logger;


    public RabbitMQConsumerHostedService(IRabbitMqConsumer rabbitMqConsumer,
        ILogger<RabbitMQConsumerHostedService> logger)
    {
        _rabbitMqConsumer = rabbitMqConsumer ?? throw new ArgumentException(nameof(rabbitMqConsumer));
        _logger =logger ?? throw new ArgumentException(nameof(logger));
    }


    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting RabbitMQ consumer...");

        await _rabbitMqConsumer.Consumer(
               queueName: "task_queue",
               consumerTag: "f360_exchange",
               cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping RabbitMQ consumer...");

    }
}
