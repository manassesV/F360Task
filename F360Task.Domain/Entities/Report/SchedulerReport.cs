namespace F360Task.Domain.Entities.Report;


public class SchedulerReport : Scheduler, IAggregateRoot
{
    public SchedulerReport(string reportType,
        string format,
        DateTime periodStart,
        DateTime periodEnd)
    {
        Type = SchedulerType.Report;
        ReportType = reportType;
        Format = format;
        PeriodStart = PeriodStart;
        PeriodEnd = PeriodEnd;

        AddDomainEvent(new SchedullerCreatedDomainEvent(ReportType, Format, PeriodStart, PeriodEnd));
    }
    public string ReportType { get; set; }
    public string Format { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
}
