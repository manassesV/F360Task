namespace F360Task.Report.EventBusRabbitMQ.Publisher;

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

    private async Task EnsureQueueBoundAsync (
        string exchange,
        string queueName,
        string routingKey,
        IChannel channel,
        string exchangeType = ExchangeType.Direct,
        bool durable = true,
        CancellationToken cancellationToken = default)
    {
       await  channel.ExchangeDeclareAsync(
            exchange: exchange,
            type: exchangeType,
            durable: durable);

       await channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

       await channel.QueueBindAsync(
            queue: queueName,
            exchange: exchange,
            routingKey: routingKey,
            arguments: null);


    }

    public async Task Publish(
        string exchange,
        string queueName,
        string routingKey,
        bool mandatory,
        string MessageId,
        ReadOnlyMemory<byte> body,
        CancellationToken cancellationToken)
    {


        var channel = await _rabbitMQConnectionProvider.GetPublishChannel();

        await EnsureQueueBoundAsync(exchange, queueName, routingKey, channel, cancellationToken: cancellationToken);
    

        try
        {
            await channel.TxSelectAsync();

            var props = new BasicProperties();
            props.Persistent = true;
            props.MessageId = MessageId;


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
        finally
        {
            _logger.LogInformation("Closing Channel.");

            await channel.CloseAsync();
        }

    }
}
