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

    public IChannel? Channel =>  _channel;
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


        try
        {
        
            _logger.LogInformation("RabbitMQ: Message delivered. ConsumerTag: {ConsumerTag}, DeliveryTag: {DeliveryTag}, Redelivered: {Redelivered}, Exchange: {Exchange}, RoutingKey: {RoutingKey}",
           consumerTag, deliveryTag, redelivered, exchange, routingKey);

            var messageId = properties.MessageId ?? $"{consumerTag}-{deliveryTag}";

            var alreadyExists = await _inboxMessageRepository.ExistAsync(messageId, cancellationToken);

            if (alreadyExists)
            {
                _logger.LogInformation("RabbitMQ: Skipping already processed message. MessageId: {MessageId}", messageId);
                return;
            }

            _logger.LogInformation("RabbitMQ: Processing message. ConsumerTag: {ConsumerTag}, MessageId: {MessageId}, Exchange: {Exchange}, RoutingKey: {RoutingKey}",
                 consumerTag, messageId, exchange, routingKey);

            var messageText = Encoding.UTF8.GetString(body.Span);
            var inboxMessage = new InboxMessage(messageId, messageText);

           
            await _inboxMessageRepository.AddAsync(inboxMessage);
            await _inboxMessageRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
           

            if (Channel.IsOpen)
            {
               await Channel.BasicAckAsync(deliveryTag, multiple: false, cancellationToken);
            }
            else
            {
                _logger.LogWarning("Canal RabbitMQ fechado ao tentar dar ACK");
            }


        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RabbitMQ: Error while processing message {MessageId}", properties.MessageId);

            if (Channel.IsOpen)
            {
                await Channel.BasicNackAsync(deliveryTag, multiple: false, true, cancellationToken);
            }
        }
    }

    public async Task HandleChannelShutdownAsync(object channel, ShutdownEventArgs reason)
    {
        _logger.LogInformation("RabbitMQ: Channel shutdown detected. Channel: {Channel}, Reason: {Reason}", channel, reason.ReplyText);
    }
}
 