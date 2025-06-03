namespace F360Task.Report.Infrastructure.Infrastructure.Contexts.Idempotent;

public class ClienteRequestRepository : IClienteRequestRepository
{
    private readonly ReportDbContext _context;

    public ClienteRequestRepository(ReportDbContext context)
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
