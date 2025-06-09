namespace F360Task.Report.Infrastructure.Outbox;

public interface IOutboxMessageRepository
{
    IUnitOfWork UnitOfWork { get; }

    Task AddAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken);
    Task UpdateAsync(OutboxMessage outboxMessage);
    Task<List<OutboxMessage>> FindAllAsync(bool processed,
         DateTime now,
         TimeSpan lockDuration,
         CancellationToken cancellationToken);
}
