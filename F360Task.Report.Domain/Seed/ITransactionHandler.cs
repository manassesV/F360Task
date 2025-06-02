namespace F360Task.Report.Domain.Seed;
public interface ITransactionHandler<T>
{
    Task<bool> ExecuteAsync(Func<T, Task> action, CancellationToken cancellationToken = default);
}
