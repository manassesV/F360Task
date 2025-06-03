namespace F360Task.Report.EventBusRabbitMQ.BackgroundServices;

public class RabbitMQInitializerHostedService : IHostedService
{
    private readonly IRabbitMQConnectionProvider _provider;
    private readonly ILogger<RabbitMQInitializerHostedService> _logger;

    public RabbitMQInitializerHostedService(
        IRabbitMQConnectionProvider provider,
        ILogger<RabbitMQInitializerHostedService> logger)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return;

        _logger.LogInformation("Starting RabbitMQ connection initialization...");

        await _provider.InitializeAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping RabbitMQ connection...");

        await _provider.CloseAsync();
    }
}
