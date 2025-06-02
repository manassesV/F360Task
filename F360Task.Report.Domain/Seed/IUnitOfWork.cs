namespace F360Task.Report.Domain.Seed;
public interface IUnitOfWork: IDisposable
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
