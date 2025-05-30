namespace F360Task.Infrastructure.Infrastructure.Inbox;

public class OutboxMessageRepository : IInboxMessageRepository
{
    private readonly ApplicationDbContext _context;

    public OutboxMessageRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task AddAsync(InboxMessage inboxMessage)
    {
        _context.InboxMessage.Add(inboxMessage);
    }

    public Task<bool> ExistAsync(Guid id, CancellationToken cancellationToken)
    {
        var request = _context.InboxMessage
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        return request != null ? Task.FromResult(true) : Task.FromResult(false);
    }
}
