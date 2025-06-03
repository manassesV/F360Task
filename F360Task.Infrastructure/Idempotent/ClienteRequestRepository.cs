using SharpCompress.Common;

namespace F360Task.Infrastructure.Infrastructure.Contexts.Idempotent;

public class ClienteRequestRepository : IClienteRequestRepository
{
    private readonly EmailDbContext _context;

    public ClienteRequestRepository(EmailDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task AddAsync(ClienteRequest request)
    {
        _context.ClienteRequest.Add(request);
    }

    public async Task<bool> ExistAsync(Guid id, CancellationToken cancellationToken)
    {
        var request = await _context.ClienteRequest
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        return request != null;
    }
}
