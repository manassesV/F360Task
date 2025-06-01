namespace F360Task.Application.Queries.SchedulerEmail;

public class SchedulerReportViewModel
{
    public Guid Id { get; set; }
    public string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }

}
