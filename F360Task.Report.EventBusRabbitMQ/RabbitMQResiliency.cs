namespace F360Task.Report.EventBusRabbitMQ;

public class RabbitMQResiliency
{
    private ILogger<RabbitMQResiliency> _logger;

    public RabbitMQResiliency(ILogger<RabbitMQResiliency> logger)
    {
        _logger = logger;
    }


    public AsyncRetryPolicy GetRetryPolicy()
    {
        return Policy
            .Handle<Exception>()
            .Or<BrokerUnreachableException>()
            .WaitAndRetryForeverAsync(
                sleepDurationProvider: _ => TimeSpan.FromSeconds(5),
                onRetry: (exception, timespan) =>
                {
                    _logger.LogWarning($"Retrying in {timespan.TotalSeconds}s due to: {exception.Message}");
                });
    }
}
