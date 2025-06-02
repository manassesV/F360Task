
namespace F360Task.Report.Application.Queries.SchedulerReport;

public interface ISchedulerReportQueries
{
    Task<List<SchedulerReportViewModel>> FindAllAsync();
    Task<SchedulerReportViewModel> FindByIdAsync(Guid id);
}