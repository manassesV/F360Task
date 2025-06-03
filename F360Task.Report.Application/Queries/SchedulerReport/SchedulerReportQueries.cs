namespace F360Task.Report.Application.Queries.SchedulerReport;

public class SchedulerReportQueries : ISchedulerReportQueries
{
    private ReportDbContext _context;

    public SchedulerReportQueries(ReportDbContext context)
    {
        _context = context;
    }

    public async Task<List<SchedulerReportViewModel>> FindAllAsync()
    {
        return await _context.SchedulerReports.AsNoTracking().Select(
            c => new SchedulerReportViewModel
            {
                Format = c.Format,
                PeriodStart = c.PeriodStart,
                PeriodEnd = c.PeriodEnd,
                ReportType = c.ReportType
            }).ToListAsync();
    }

    public async Task<SchedulerReportViewModel> FindByIdAsync(Guid id)
    {
        var scheduleReports = await _context.SchedulerReports.AsNoTracking().FirstOrDefaultAsync(p => p.Id.Equals(id));

        if (scheduleReports is null)
            throw new KeyNotFoundException();

        return new SchedulerReportViewModel
        {
            Format = scheduleReports.Format,
            PeriodStart = scheduleReports.PeriodStart,
            PeriodEnd = scheduleReports.PeriodEnd,
            ReportType = scheduleReports.ReportType

        };
    }
}
