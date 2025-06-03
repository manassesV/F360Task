namespace F360Task.Report.EventBusRabbitMQ.Consumer;

public class RabbitAsyncConsumer : IRabbitAsyncConsumer
{
    private readonly ILogger<RabbitAsyncConsumer> _logger;
    private readonly IInboxMessageRepository _inboxMessageRepository;
    private readonly IServiceScopeFactory _scopeFactory;


    public RabbitAsyncConsumer(IRabbitMQConnectionProvider rabbitMQConnectionProvider,
        ILogger<RabbitAsyncConsumer> logger,
        IInboxMessageRepository inboxMessageRepository,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _inboxMessageRepository = inboxMessageRepository;
        _scopeFactory = scopeFactory;
    }

    public IChannel? Channel => _channel;
    private IChannel _channel;

    public void SetChannel(IChannel channel)
    {
        _channel = channel;
    }

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
        using var scope = _scopeFactory.CreateScope();
        var transactionHandler = scope.ServiceProvider.GetRequiredService<ITransactionHandler<IClientSessionHandle>>();

        var messageId = properties.MessageId ?? $"{consumerTag}-{deliveryTag}";

        try
        {
            _logger.LogInformation("RabbitMQ: Message received. ConsumerTag: {ConsumerTag}, DeliveryTag: {DeliveryTag}, Redelivered: {Redelivered}, Exchange: {Exchange}, RoutingKey: {RoutingKey}",
                consumerTag, deliveryTag, redelivered, exchange, routingKey);

            var alreadyExists = await _inboxMessageRepository.ExistAsync(messageId, cancellationToken);
            if (alreadyExists)
            {
                _logger.LogInformation("RabbitMQ: Skipping already processed message. MessageId: {MessageId}", messageId);
                await TryAckAsync(deliveryTag, messageId, cancellationToken);
                return;
            }

            _logger.LogInformation("RabbitMQ: Processing message. ConsumerTag: {ConsumerTag}, MessageId: {MessageId}, Exchange: {Exchange}, RoutingKey: {RoutingKey}",
                consumerTag, messageId, exchange, routingKey);

            var messageText = Encoding.UTF8.GetString(body.Span);
            var inboxMessage = new InboxMessage(messageId, messageText);

            await transactionHandler.ExecuteAsync(async session =>
            {
                await _inboxMessageRepository.AddAsync(inboxMessage);
                await _inboxMessageRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            });

            await TryAckAsync(deliveryTag, messageId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RabbitMQ: Error while processing message. MessageId: {MessageId}", messageId);
            await TryNackAsync(deliveryTag, messageId, cancellationToken);
        }
    }

    public async Task HandleChannelShutdownAsync(object channel, ShutdownEventArgs reason)
    {
        _logger.LogInformation("RabbitMQ: Channel shutdown detected. Channel: {Channel}, Reason: {Reason}", channel, reason.ReplyText);
    }


    private async Task TryAckAsync(ulong deliveryTag, string messageId, CancellationToken cancellationToken)
    {
        try
        {
            if (Channel.IsOpen)
            {
                await Channel.BasicAckAsync(deliveryTag, false, cancellationToken);
                _logger.LogInformation("RabbitMQ: ACK sent successfully. MessageId: {MessageId}", messageId);
            }
            else
            {
                _logger.LogCritical("RabbitMQ: Channel is closed when attempting to ACK. MessageId: {MessageId}", messageId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RabbitMQ: Failed to send ACK. MessageId: {MessageId}", messageId);
        }
    }

    private async Task TryNackAsync(ulong deliveryTag, string messageId, CancellationToken cancellationToken)
    {
        try
        {
            if (Channel.IsOpen)
            {
                await Channel.BasicNackAsync(deliveryTag, false, requeue: true, cancellationToken);
                _logger.LogInformation("RabbitMQ: NACK sent with requeue. MessageId: {MessageId}", messageId);
            }
            else
            {
                _logger.LogCritical("RabbitMQ: Channel is closed when attempting to NACK. MessageId: {MessageId}", messageId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RabbitMQ: Failed to send NACK. MessageId: {MessageId}", messageId);
        }
    }
}
