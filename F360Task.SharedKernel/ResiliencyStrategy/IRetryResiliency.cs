namespace F360Task.SharedKernel.ResiliencyStrategy;

public interface IRetryResiliency
{
    Task<T> ExecuteAsync<T>(Func< Task<T>> action, CancellationToken cancellationToken);
    Task ExecuteAsync(Func< Task> action, CancellationToken cancellationToken);
}