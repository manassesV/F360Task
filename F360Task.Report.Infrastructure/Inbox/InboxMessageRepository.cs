namespace F360Task.Report.Infrastructure.Inbox;

public class InboxMessageRepository : IInboxMessageRepository
{
    private readonly ApplicationDbContext _context;
    public IUnitOfWork UnitOfWork => _context;


    public InboxMessageRepository(ApplicationDbContext context)
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
            .AsNoTracking()
            .Where(p => p.Processed == processed)
            .OrderBy(c => c.ProcessedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(InboxMessage inboxMessage)
    {
        _context.InboxMessage.Update(inboxMessage);
    }
}

