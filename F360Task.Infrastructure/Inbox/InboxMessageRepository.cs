namespace F360Task.Infrastructure.Inbox;

public class InboxMessageRepository : IInboxMessageRepository
{
    private readonly EmailDbContext _context;
    public IUnitOfWork UnitOfWork => _context;
    private readonly ITransactionHandler<IClientSessionHandle> _transactionHandler;



    public InboxMessageRepository(EmailDbContext context,
        ITransactionHandler<IClientSessionHandle> transactionHandler)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _transactionHandler = transactionHandler ?? throw new ArgumentNullException(nameof(transactionHandler));
    }


    public async Task AddAsync(InboxMessage inboxMessage)
    {
        await _transactionHandler.ExecuteAsync(async (session) =>
        {
            _context.InboxMessage.Add(inboxMessage);

        });
    }

    public async Task<bool> ExistAsync(string id, CancellationToken cancellationToken)
    {
        var exists = await _context.InboxMessage
            .AsNoTracking()
            .AnyAsync(r => r.Id == id, cancellationToken);

        return exists;
    }

    public async Task<List<InboxMessage>> FindAllAsync(bool processed,
        DateTime now,
        TimeSpan lockDuration,
        CancellationToken cancellationToken)
    {

        List<InboxMessage> lockedMessage = new();

        await _transactionHandler.ExecuteAsync(async (session) =>
        {

            lockedMessage = await _context.InboxMessage
            .AsQueryable()
            .Where(p => p.Processed == processed &&
            (!p.LockedUntil.HasValue || p.LockedUntil < now))
            .OrderBy(c => c.ProcessedAt)
            .ToListAsync(cancellationToken);

            foreach (var inboxMessage in lockedMessage)
            {
                inboxMessage.ChangeToLocked(now, lockDuration);
            }

            _context.InboxMessage.UpdateRange(lockedMessage);
            await _context.SaveChangesAsync(cancellationToken);

        });

        return lockedMessage;

    }

    public async Task UpdateAsync(InboxMessage inboxMessage)
    {
        await _transactionHandler.ExecuteAsync(async (session) =>
        {
            _context.InboxMessage.Update(inboxMessage);
        });
    }
}

