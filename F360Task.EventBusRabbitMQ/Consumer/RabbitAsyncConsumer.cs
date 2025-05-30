namespace F360Task.EventBusRabbitMQ.Consumer;

public class RabbitAsyncConsumer : IRabbitAsyncConsumer
{
    private readonly IConnection _connection;
    private readonly ILogger<RabbitAsyncConsumer> _logger;

    public RabbitAsyncConsumer(IConnection connection,
        ILogger<RabbitAsyncConsumer> logger)
    {
        _connection = connection;
        _logger = logger;
    }

    public IChannel? Channel => _connection.CreateChannelAsync().GetAwaiter().GetResult();

    public async Task HandleBasicCancelAsync(string consumerTag, CancellationToken cancellationToken = default)
    {

        _logger.LogInformation("BasicCancel {consumerTag}", consumerTag);
    }

    public async Task HandleBasicCancelOkAsync(string consumerTag, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("CancelOk {consumerTag}", consumerTag);
    }

    public async Task HandleBasicConsumeOkAsync(string consumerTag, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("ConsumeOk {consumerTag}", consumerTag);

    }

    public async Task HandleBasicDeliverAsync(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IReadOnlyBasicProperties properties, ReadOnlyMemory<byte> body, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deliver {consumerTag}", consumerTag);

    }

    public async Task HandleChannelShutdownAsync(object channel, ShutdownEventArgs reason)
    {
        _logger.LogInformation("ChannelShutdown {consumerTag}", channel);
    }
}
