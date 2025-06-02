
using RabbitMQ.Client;

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
        string exchange,
        string queueName,
        string consumerTag,
        CancellationToken cancellationToken)
    {


        var connection = _rabbitMQConnectionProvider.GetConnection();

        var channel = await connection.CreateChannelAsync();
        _rabbitAsyncConsumer.SetChannel(channel); // ← canal correto sendo passado

        try
        {

            await EnsureQueueBoundAsync(exchange, queueName, channel);

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
            _logger.LogError(ex, "Failed to consume message. Rolling back.");

            throw;

        }

    }

    private async Task EnsureQueueBoundAsync(string exchange,
        string queueName,
        IChannel channel,
        string exchangeType = ExchangeType.Direct,
        bool durable = true)
    {
        await channel.ExchangeDeclareAsync(
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
             routingKey: "",
             arguments: null);
    }
}
