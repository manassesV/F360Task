namespace F360Task.Report.EventBusRabbitMQ.Publisher;

public class RabbitMQPublisherHostedService : BackgroundService
{
    private readonly ILogger<RabbitMQPublisherHostedService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _pollingInterval = TimeSpan.FromSeconds(30);
    private const int MaxRetryCount = 3;

    public RabbitMQPublisherHostedService(
        IServiceProvider serviceProvider,
        ILogger<RabbitMQPublisherHostedService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Outbox Processor Service starting");
        using var timer = new PeriodicTimer(_pollingInterval);
        var retryCount = 0;

        while (await timer.WaitForNextTickAsync(cancellationToken))
        {
            try
            {
                await using var scope = _serviceProvider.CreateAsyncScope();

                var repository = scope.ServiceProvider.GetRequiredService<IOutboxMessageRepository>();
                var publisher = scope.ServiceProvider.GetRequiredService<IRabbitMqPublisher>();
                var transactionHandler = scope.ServiceProvider.GetRequiredService<ITransactionHandler<IClientSessionHandle>>();

                await ProcessMessagesAsync(repository, publisher, transactionHandler, cancellationToken);
                retryCount = 0;
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Outbox processing was cancelled");
                break;
            }
            catch (Exception ex) when (retryCount++ < MaxRetryCount)
            {
                _logger.LogError(ex, "Error processing outbox messages (Retry {RetryCount}/{MaxRetries})", retryCount, MaxRetryCount);
                await Task.Delay(5000, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Max retries reached for outbox processing");
                throw;
            }
        }

        _logger.LogInformation("Outbox Processor Service stopped");
    }

    private async Task ProcessMessagesAsync(
        IOutboxMessageRepository repository,
        IRabbitMqPublisher publisher,
        ITransactionHandler<IClientSessionHandle> transactionHandler,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var lockDuration = TimeSpan.FromSeconds(30);

        var messages = await repository.FindAllAsync(processed: false,
            now,
            lockDuration,
            cancellationToken);

        if (messages == null || !messages.Any())
        {
            _logger.LogInformation("No unprocessed outbox messages found.");
            return;
        }

        _logger.LogInformation("Processing {Count} messages...", messages.Count());

        foreach (var message in messages)
        {

            try
            {
                using var _ = _logger.BeginScope("Processing message {MessageId}", message.Id);
                await ProcessSingleMessageAsync(message, repository, publisher, transactionHandler, cancellationToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Failed to process message {MessageId}", message.Id);

                await MarkMessageAsFailedAsync(message, repository, transactionHandler, cancellationToken);

            }
        }

    }

    private async Task ProcessSingleMessageAsync(
        OutboxMessage message,
        IOutboxMessageRepository repository,
        IRabbitMqPublisher publisher,
        ITransactionHandler<IClientSessionHandle> transactionHandler,
        CancellationToken cancellationToken)
    {
        try
        {
            if (message.Payload is null)
            {
                _logger.LogWarning("Message has null payload");


                await MarkMessageAsFailedAsync(message, repository, transactionHandler, cancellationToken);

                return;
            }

            var payload = Encoding.UTF8.GetBytes(message.Payload);

            await publisher.Publish(
                exchange: message.Exchange,
                queueName: message.Queue,
                routingKey: message.Queue,
                mandatory: true,
                MessageId: message.Id.ToString(),
                body: payload,
                cancellationToken: cancellationToken);


            await MarkMessageAsProcessedAsync(message, repository, transactionHandler, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process message {MessageId}", message.Id);

            await MarkMessageAsFailedAsync(message, repository, transactionHandler, cancellationToken);
        }
    }

    private async Task MarkMessageAsProcessedAsync(
    OutboxMessage message,
    IOutboxMessageRepository repository,
    ITransactionHandler<IClientSessionHandle> transactionHandler,
    CancellationToken cancellationToken)
    {
        await transactionHandler.ExecuteAsync(async (session) =>
        {
            message.ChangeToProcessed();
            await repository.UpdateAsync(message);
        });
    }

    private async Task MarkMessageAsFailedAsync(
        OutboxMessage message,
        IOutboxMessageRepository repository,
        ITransactionHandler<IClientSessionHandle> transactionHandler,
        CancellationToken cancellationToken)

    {
        await transactionHandler.ExecuteAsync(async (session) =>
        {
            message.ChangeToFailed();
            await repository.UpdateAsync(message);
        });
    }


}
