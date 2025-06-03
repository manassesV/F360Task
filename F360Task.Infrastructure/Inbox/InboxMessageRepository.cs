namespace F360Task.Infrastructure.Inbox;

public class InboxMessageRepository : IInboxMessageRepository
{
    private readonly EmailDbContext _context;
    public IUnitOfWork UnitOfWork => _context;


    public InboxMessageRepository(EmailDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }


    public async Task AddAsync(InboxMessage inboxMessage)
    {
        _context.InboxMessage.Add(inboxMessage);
    }

    public async Task<bool> ExistAsync(string id, CancellationToken cancellationToken)
    {
        var exists = await _context.InboxMessage
            .AsNoTracking()
            .AnyAsync(r => r.Id == id, cancellationToken);

        return exists;
    }

    public Task<List<InboxMessage>> FindAllAsync(bool processed, CancellationToken cancellationToken)
    {
        return _context.InboxMessage
            .Where(p => p.Processed == processed)
            .OrderBy(c => c.ProcessedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(InboxMessage inboxMessage)
    {
        _context.InboxMessage.Update(inboxMessage);
    }
}

