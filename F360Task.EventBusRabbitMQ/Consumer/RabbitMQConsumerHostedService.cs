namespace F360Task.EventBusRabbitMQ.Consumer;

public class RabbitMQConsumerHostedService : IHostedService
{

    private readonly ILogger<RabbitMQConsumerHostedService> _logger;
    private readonly IRetryResiliency _retryResiliency;
    private readonly IServiceScopeFactory _serviceScopeFactory;


    public RabbitMQConsumerHostedService(IServiceScopeFactory serviceScopeFactory,
        ILogger<RabbitMQConsumerHostedService> logger,
        IRetryResiliency retryResiliency)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger =logger ?? throw new ArgumentException(nameof(logger));
        _retryResiliency = retryResiliency;
    }


    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting RabbitMQ consumer...");

        try
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var _rabbitMqConsumer = scope.ServiceProvider.GetRequiredService<IRabbitMqConsumer>();

            await _retryResiliency.ExecuteAsync(async () =>
            {
                await _rabbitMqConsumer.Consumer(
                    "Email",
                    queueName: "CreateSchedullerEmail",
                    consumerTag: "f360_exchange",
                    cancellationToken);
            }, cancellationToken);

            _logger.LogInformation("RabbitMQ consumer started successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RabbitMQ consumer failed to start after retries.");
            throw; 
        }

    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping RabbitMQ consumer...");

    }
}
