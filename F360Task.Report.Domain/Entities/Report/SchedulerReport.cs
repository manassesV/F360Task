namespace F360Task.Report.Domain.Entities.Report;


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
        PeriodStart = periodStart;
        PeriodEnd = periodEnd;

        //AddDomainEvent(new SchedullerCreatedDomainEvent(ReportType, Format, PeriodStart, PeriodEnd));
    }
    public string ReportType { get; private set; }
    public string Format { get; private set; }
    public DateTime PeriodStart { get; private set; }
    public DateTime PeriodEnd { get; private set; }
}
