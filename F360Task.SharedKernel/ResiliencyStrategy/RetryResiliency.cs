﻿namespace F360Task.SharedKernel.ResiliencyStrategy;

public class RetryResiliency : IRetryResiliency
{
    private readonly ILogger<RetryResiliency> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;
    public RetryResiliency(
         AsyncRetryPolicy retryPolicy,
        ILogger<RetryResiliency> logger)
    {
        _retryPolicy = retryPolicy ?? throw new ArgumentNullException(nameof(retryPolicy));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    public async Task<T> ExecuteAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken)
    {
        var result = await _retryPolicy.ExecuteAndCaptureAsync(action);

        if (result.Outcome == OutcomeType.Successful && result.Result is not null)
        {
            _logger.LogInformation("Operation succeeded.");

            return result.Result;
        }
        else
        {
            _logger.LogError($"Operation failed after retries. Exception: {result.FinalException?.Message}");
            throw result.FinalException ?? new Exception("Unknown failure.");
        }
    }

    public async Task ExecuteAsync(Func<Task> action, CancellationToken cancellationToken)
    {
        var result = await _retryPolicy.ExecuteAndCaptureAsync(action);


        if (result.Outcome == OutcomeType.Successful)
        {
            _logger.LogInformation("Operation succeeded.");
        }
        else
        {
            _logger.LogError($"Operation failed after retries. Exception: {result.FinalException?.Message}");
            throw result.FinalException ?? new Exception("Unknown failure.");
        }
    }
}
