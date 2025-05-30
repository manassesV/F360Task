namespace F360Task.Domain.Seed;
public interface IUnitOfWork: IDisposable
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
