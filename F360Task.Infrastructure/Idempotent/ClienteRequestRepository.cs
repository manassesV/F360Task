namespace F360Task.Infrastructure.Infrastructure.Contexts.Idempotent;

public class ClienteRequestRepository : IInboxMessageRepository
{
    private readonly ApplicationDbContext _context;

    public ClienteRequestRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task AddAsync(ClienteRequest request)
    {
        _context.ClienteRequest.Add(request);
    }

    public Task<bool> ExistAsync(Guid id, CancellationToken cancellationToken)
    {
        var request = _context.ClienteRequest
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        return request != null ? Task.FromResult(true) : Task.FromResult(false);
    }
}
