namespace F360Task.Infrastructure.Infrastructure.Outbox;

public interface IOutboxMessageRepository
{
    Task AddAsync(OutboxMessage outboxMessage);
    Task<bool> ExistAsync(Guid id, CancellationToken cancellationToken);
}
