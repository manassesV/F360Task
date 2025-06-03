namespace F360Task.Application.Queries.SchedulerEmail;

public class SchedulerEmailQueries : ISchedulerEmailQueries
{
    private EmailDbContext _context;

    public SchedulerEmailQueries(EmailDbContext context)
    {
        _context = context;
    }

    public async Task<List<SchedulerReportViewModel>> FindAllAsync()
    {
        return await _context.SchedulerEmails.AsNoTracking().Select(
            c => new SchedulerReportViewModel
            {
                Id = c.Id,
                To = c.To,
                Body = c.Body,
                Subject = c.Subject
            }).ToListAsync();
    }

    public async Task<SchedulerReportViewModel> FindByIdAsync(Guid id)
    {
        var schedulerEmail = await _context.SchedulerEmails.AsNoTracking().FirstOrDefaultAsync(c => c.Id.Equals(id));

        if (schedulerEmail is null)
            throw new KeyNotFoundException();


        return new SchedulerReportViewModel
        {
            Id = schedulerEmail.Id,
            Body = schedulerEmail.Body,
            Subject = schedulerEmail.Subject,
            To = schedulerEmail.To
        };
    }
}
