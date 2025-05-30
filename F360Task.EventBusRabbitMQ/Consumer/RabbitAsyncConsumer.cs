using F360Task.Domain.Seed;
using MongoDB.Driver;
using System.Text;
using System.Text.Unicode;

namespace F360Task.EventBusRabbitMQ.Consumer;

public class RabbitAsyncConsumer : IRabbitAsyncConsumer
{
    private readonly IRabbitMQConnectionProvider _rabbitMQConnectionProvider;
    private readonly ILogger<RabbitAsyncConsumer> _logger;
    private readonly IConnection _connection;
    private readonly IInboxMessageRepository _inboxMessageRepository;
    private readonly ITransactionHandler<IClientSessionHandle> _transactionHandler;

    public RabbitAsyncConsumer(IRabbitMQConnectionProvider rabbitMQConnectionProvider,
        ILogger<RabbitAsyncConsumer> logger,
        IInboxMessageRepository inboxMessageRepository,
        ITransactionHandler<IClientSessionHandle> transactionHandler)
    {
        _rabbitMQConnectionProvider = rabbitMQConnectionProvider;
        //_connection = _rabbitMQConnectionProvider.GetConnection();
        _logger = logger;
        _inboxMessageRepository = inboxMessageRepository;
        _transactionHandler = transactionHandler;
    }

    public IChannel? Channel => null;

    public async Task HandleBasicCancelAsync(string consumerTag, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("RabbitMQ: Consumer cancellation received. ConsumerTag: {ConsumerTag}", consumerTag);
    }

    public async Task HandleBasicCancelOkAsync(string consumerTag, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("RabbitMQ: Consumer cancellation acknowledged. ConsumerTag: {ConsumerTag}", consumerTag);
    }

    public async Task HandleBasicConsumeOkAsync(string consumerTag, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("RabbitMQ: Consumer registered successfully. ConsumerTag: {ConsumerTag}", consumerTag);
    }

    public async Task HandleBasicDeliverAsync(
        string consumerTag,
        ulong deliveryTag,
        bool redelivered,
        string exchange,
        string routingKey,
        IReadOnlyBasicProperties properties,
        ReadOnlyMemory<byte> body,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("RabbitMQ: Message delivered. ConsumerTag: {ConsumerTag}, DeliveryTag: {DeliveryTag}, Redelivered: {Redelivered}, Exchange: {Exchange}, RoutingKey: {RoutingKey}",
           consumerTag, deliveryTag, redelivered, exchange, routingKey);

            var messageId = properties.MessageId;

            if (messageId is null)
                _logger.LogInformation("MessageId is goin null");

            var alredyExist = await _inboxMessageRepository.ExistAsync(messageId, cancellationToken);

            if (alredyExist)
            {
                _logger.LogInformation("RabbitMQ: Skipping already processed message. MessageId: {MessageId}", messageId);
                return;
            }

            _logger.LogInformation("RabbitMQ: Processing message. ConsumerTag: {ConsumerTag}, MessageId: {MessageId}, Exchange: {Exchange}, RoutingKey: {RoutingKey}",
                 consumerTag, messageId, exchange, routingKey);

            var messageText = Encoding.UTF8.GetString(body.Span);
            var inboxMessage = new InboxMessage(messageId, messageText);

            await _transactionHandler.ExecuteAsync((clienthandler) =>
            {
                _inboxMessageRepository.AddAsync(inboxMessage);

                Channel.BasicAckAsync(deliveryTag, true, cancellationToken);

                return Task.CompletedTask;
            });

        }
        catch (Exception ex)
        {

            _logger.LogError("", ex.Message);
        }
    }

    public async Task HandleChannelShutdownAsync(object channel, ShutdownEventArgs reason)
    {
        _logger.LogInformation("RabbitMQ: Channel shutdown detected. Channel: {Channel}, Reason: {Reason}", channel, reason.ReplyText);
    }
}
 