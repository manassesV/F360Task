namespace F360Task.Application.Queries.SchedulerReport;

public class SchedulerReportViewModel
{
    public string ReportType { get; set; }
    public string Format { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }

}
