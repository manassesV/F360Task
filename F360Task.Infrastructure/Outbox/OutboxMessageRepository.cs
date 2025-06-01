namespace F360Task.Infrastructure.Outbox;

public class OutboxMessageRepository : IOutboxMessageRepository
{
    private readonly ApplicationDbContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public OutboxMessageRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task AddAsync(OutboxMessage outboxMessage)
    {
        _context.OutboxMessage.Add(outboxMessage);
    }

    public Task<bool> ExistAsync(Guid id, CancellationToken cancellationToken)
    {
        var request = _context.OutboxMessage
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        return request != null ? Task.FromResult(true) : Task.FromResult(false);
    }

    public async Task<List<OutboxMessage>> FindAllAsync(bool processed,
        CancellationToken cancellationToken)
    {
        return await _context.OutboxMessage
            .AsNoTracking()
            .Where(p => p.Processed == processed)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(OutboxMessage outboxMessage)
    {
        _context.OutboxMessage.Update(outboxMessage);
    }
}
