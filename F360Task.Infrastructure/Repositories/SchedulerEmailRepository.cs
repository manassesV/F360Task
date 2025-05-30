namespace F360Task.Infrastructure.Repositories;

public class SchedulerEmailRepository : ISchedulerEmailRepository
{
    private readonly ApplicationDbContext _context;

    public SchedulerEmailRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public IUnitOfWork UnitOfWork => throw new NotImplementedException();

    public async Task AddAsync(SchedulerEmail entity)
    {
        _context.SchedulerEmails.Add(entity);
    }

    public async Task DeleteAsync(SchedulerEmail entity)
    {
        _context.SchedulerEmails.Remove(entity);

    }

    public async Task<IEnumerable<SchedulerEmail>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.SchedulerEmails.ToListAsync(cancellationToken);

    }

    public async Task<SchedulerEmail> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.SchedulerEmails.FirstOrDefaultAsync(ct => ct.Id.Equals(id), cancellationToken);

    }

    public async Task UpdateAsync(SchedulerEmail entity)
    {
        _context.SchedulerEmails.Update(entity);
    }
}
