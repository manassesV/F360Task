namespace F360Task.Report.Infrastructure.Infrastructure.Contexts.Idempotent;
public interface IRequestManager
{
    Task CreateRequestForCommandAsync<T>(Guid id, CancellationToken cancellationToken);
}
