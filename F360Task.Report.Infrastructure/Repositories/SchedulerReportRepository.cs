namespace F360Task.Report.Infrastructure.Repositories;

public class SchedulerReportRepository : ISchedulerReportRepository
{
    private readonly ApplicationDbContext _context;

    public SchedulerReportRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public IUnitOfWork UnitOfWork => _context;

    public async Task AddAsync(SchedulerReport entity)
    {
        _context.SchedulerReports.Add(entity);
    }

    public async Task DeleteAsync(SchedulerReport entity)
    {
        _context.SchedulerReports.Remove(entity);
    }

    public async Task<IEnumerable<SchedulerReport>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.SchedulerReports.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<SchedulerReport> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.SchedulerReports.FirstOrDefaultAsync(ct => ct.Id.Equals(id), cancellationToken);
    }

    public Task UpdateAsync(SchedulerReport entity)
    {
        throw new NotImplementedException();
    }
}
