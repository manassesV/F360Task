
namespace F360Task.Application.Queries.SchedulerReport
{
    public interface ISchedulerReportQueries
    {
        Task<List<SchedulerReportViewModel>> FindAllAsync();
        Task<SchedulerReportViewModel> FindByIdAsync(Guid id);
    }
}