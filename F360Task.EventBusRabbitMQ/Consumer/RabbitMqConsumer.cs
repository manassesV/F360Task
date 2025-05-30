namespace F360Task.EventBusRabbitMQ.Consumer;

public class RabbitMqConsumer : IRabbitMqConsumer
{
    private readonly IRabbitMQConnectionProvider _rabbitMQConnectionProvider;
    private readonly ILogger<RabbitMqConsumer> _logger;
    private readonly IRabbitAsyncConsumer _rabbitAsyncConsumer;
    public RabbitMqConsumer(IRabbitMQConnectionProvider rabbitMQConnectionProvider,
        ILogger<RabbitMqConsumer> logger,
        IRabbitAsyncConsumer rabbitAsyncConsumer)
    {
        _rabbitMQConnectionProvider = rabbitMQConnectionProvider ?? throw new ArgumentException(nameof(_rabbitMQConnectionProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _rabbitAsyncConsumer = rabbitAsyncConsumer ?? throw new ArgumentNullException(nameof(rabbitAsyncConsumer));
    }

    public async Task Consumer(
        string queueName,
        string consumerTag,
        CancellationToken cancellationToken)
    {


        using var connection = _rabbitMQConnectionProvider.GetConnection();

        using var channel = await connection.CreateChannelAsync();

        try
        {

             await channel.BasicConsumeAsync(
                    queue: queueName,
                    autoAck: false,
                    consumerTag: consumerTag,
                    noLocal: false,
                    exclusive: false,
                    arguments: null,
                    consumer: _rabbitAsyncConsumer,
                    cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message. Rolling back.");

            throw;
        }

    }
}
