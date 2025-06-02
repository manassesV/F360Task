namespace F360Task.Report.Application.Commands;

public record UpdateSchedulerReportCommand(Guid id, string ReportType, string Format, DateTime PeriodStart, DateTime PeriodEnd) : IRequest<Result>;
