namespace F360Task.Report.Infrastructure.Outbox;

public interface IOutboxMessageRepository
{
    IUnitOfWork UnitOfWork { get; }

    Task AddAsync(OutboxMessage outboxMessage);
    Task UpdateAsync(OutboxMessage outboxMessage);
    Task<List<OutboxMessage>> FindAllAsync(bool processed,
        CancellationToken cancellationToken);
}
