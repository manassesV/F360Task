namespace F360Task.EventBusRabbitMQ.Publisher;

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

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Outbox Processor Service starting");

        using var timer = new PeriodicTimer(_pollingInterval);
        var retryCount = 0;

        while (!stoppingToken.IsCancellationRequested &&
               await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await using (var scope = _serviceProvider.CreateAsyncScope())
                {
                    var repository = scope.ServiceProvider
                        .GetRequiredService<IOutboxMessageRepository>();
                    var publisher = scope.ServiceProvider
                        .GetRequiredService<IRabbitMqPublisher>();
                    var transactionHandler = scope.ServiceProvider
                        .GetRequiredService<ITransactionHandler<IClientSessionHandle>>();

                    await ProcessMessagesAsync(repository, publisher, transactionHandler, stoppingToken);
                    retryCount = 0; ;
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Outbox processing was cancelled");
                break;
            }
            catch (Exception ex) when (retryCount++ < MaxRetryCount)
            {
                _logger.LogError(ex, "Error processing outbox messages (Retry {RetryCount}/{MaxRetries})",
                    retryCount, MaxRetryCount);
                await Task.Delay(5000, stoppingToken);
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
        var messages = await repository.FindAllAsync(false, cancellationToken);

        if (messages.Count == 0)
        {
            _logger.LogDebug("No pending messages found");
            return;
        }

        _logger.LogInformation("Processing {MessageCount} messages", messages.Count);

        await Parallel.ForEachAsync(messages, new ParallelOptions
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = Environment.ProcessorCount
        }, async (message, ct) =>
        {
            using var _ = _logger.BeginScope("Processing message {MessageId}", message.Id);
            await ProcessSingleMessageAsync(message, repository, publisher, transactionHandler, ct);
        });
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
                await MarkMessageAsProcessed(message, repository, transactionHandler, false);
                return;
            }

            var payload = Encoding.UTF8.GetBytes(message.Payload);

            await transactionHandler.ExecuteAsync(async session =>
            {
                await publisher.Publish(
                    exchange: message.Exchange,
                    queueName: message.Queue,
                    routingKey: message.Queue,
                    mandatory: true,
                    MessageId: message.Id.ToString(),
                    body: payload,
                    cancellationToken: cancellationToken);

                await MarkMessageAsProcessed(message, repository, transactionHandler, true);
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process message");
            await MarkMessageAsProcessed(message, repository, transactionHandler, false);
        }
    }

    private async Task MarkMessageAsProcessed(
        OutboxMessage message,
        IOutboxMessageRepository repository,
        ITransactionHandler<IClientSessionHandle> transactionHandler,
        bool isSuccess)
    {
        try
        {
            await transactionHandler.ExecuteAsync(async session =>
            {
                if (isSuccess)
                {
                    message.ChangeToProcessed();
                }
                else
                {
                    message.ChangeToFailed();
                    message.IncrementRetryCount();
                }

                await repository.UpdateAsync(message);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update message status");
        }
    }
}