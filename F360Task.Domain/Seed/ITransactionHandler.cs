namespace F360Task.Domain.Seed;
public interface ITransactionHandler<T>
{
    Task<bool> ExecuteAsync(Func<T, Task> action, CancellationToken cancellationToken = default);
}
