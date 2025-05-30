namespace F360Task.Infrastructure.Infrastructure.Contexts.Idempotent;

public interface IInboxMessageRepository
{
    Task AddAsync(ClienteRequest request);
    Task<bool> ExistAsync(Guid id, CancellationToken cancellationToken);
}
