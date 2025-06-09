namespace F360Task.Report.Infrastructure.Outbox;

public class OutboxMessageRepository : IOutboxMessageRepository
{
    private readonly ReportDbContext _context;
    public IUnitOfWork UnitOfWork => _context;
    private ITransactionHandler<IClientSessionHandle> _transactionHandler;

    public OutboxMessageRepository(ReportDbContext context, ITransactionHandler<IClientSessionHandle> transactionHandler)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _transactionHandler = transactionHandler ?? throw new ArgumentNullException(nameof(transactionHandler));
    }

    public async Task AddAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken)
    {
       await  _transactionHandler.ExecuteAsync(async (session) =>
        {
            _context.OutboxMessage.Add(outboxMessage);
           await _context.SaveChangesAsync(cancellationToken);
        });
    }

    public Task<bool> ExistAsync(Guid id, CancellationToken cancellationToken)
    {
        var request = _context.OutboxMessage
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        return request != null ? Task.FromResult(true) : Task.FromResult(false);
    }

    public async Task<List<OutboxMessage>> FindAllAsync(bool processed,
        DateTime now,
        TimeSpan lockDuration,
        CancellationToken cancellationToken)
    {
        List<OutboxMessage> lockedMessage = new();


        await _transactionHandler.ExecuteAsync(async (session) =>
        {

            lockedMessage = await _context.OutboxMessage
            .AsQueryable()
            .Where(p => p.Processed == processed &&
            (!p.LockedUntil.HasValue || p.LockedUntil < now))
            .OrderBy(c => c.ProcessedAt)
            .ToListAsync(cancellationToken);

            foreach (var inboxMessage in lockedMessage)
            {
                inboxMessage.ChangeToLocked(now, lockDuration);
            }

            _context.OutboxMessage.UpdateRange(lockedMessage);
            await _context.SaveChangesAsync(cancellationToken);

        });

        return lockedMessage;
    }

    public async Task UpdateAsync(OutboxMessage outboxMessage)
    {
        await _transactionHandler.ExecuteAsync(async (session) =>
        {
            _context.OutboxMessage.Update(outboxMessage);
            await _context.SaveChangesAsync();
        });
    }
}
