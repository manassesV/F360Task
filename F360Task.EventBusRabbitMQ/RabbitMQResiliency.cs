namespace F360Task.EventBusRabbitMQ;

public static class RabbitMQResiliency
{
    public static RetryPolicy RetryPolicy =>
      Policy
          .Handle<Exception>()
          .WaitAndRetry(
              retryCount: 5,
              sleepDurationProvider: attempt => TimeSpan.FromSeconds(2),
              onRetry: (exception, timeSpan, retryCount, context) =>
              {
                  // Logging can go here (keep it minimal)
                  // e.g., Console.WriteLine($"Retry {retryCount} after {timeSpan} due to {exception.Message}");
              });
}
