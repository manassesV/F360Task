namespace F360Task.EventBusRabbitMQ.Publisher;

public class RabbitMqPublisher : IRabbitMqPublisher
{
    private readonly IRabbitMQConnectionProvider _rabbitMQConnectionProvider;
    private readonly ILogger<RabbitMqPublisher> _logger;
    public RabbitMqPublisher(IRabbitMQConnectionProvider rabbitMQConnectionProvider,
        ILogger<RabbitMqPublisher> logger)
    {
        _rabbitMQConnectionProvider = rabbitMQConnectionProvider ?? throw new ArgumentException(nameof(_rabbitMQConnectionProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Publish(
        string exchange,
        string routingKey,
        bool mandatory,
        ReadOnlyMemory<byte> body,
        CancellationToken cancellationToken)
    {


        var connection = _rabbitMQConnectionProvider.GetConnection();

        var channel = await connection.CreateChannelAsync();

        try
        {
            await channel.TxSelectAsync();

            var props = new BasicProperties();
            props.Persistent = true;


            await channel.BasicPublishAsync(
                    exchange,
                    routingKey,
                    false,
                    props,
                    body,
                    cancellationToken);

            await channel.TxCommitAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message. Rolling back.");

            await channel.TxRollbackAsync();

            throw;
        }

    }
}
