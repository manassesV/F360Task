
namespace F360Task.Application.Queries.SchedulerEmail;

public interface ISchedulerEmailQueries
{
    Task<List<SchedulerReportViewModel>> FindAllAsync();
    Task<SchedulerReportViewModel> FindByIdAsync(Guid id);
}