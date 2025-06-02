namespace F360Task.Domain.Seed;
public interface ITransactionHandler<T>
{
    Task ExecuteAsync(Func<T, Task> action, CancellationToken cancellationToken = default)
}
