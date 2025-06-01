namespace F360Task.Application.Commands;

public record UpdateSchedulerReportCommand(Guid id, string ReportType, string Format, DateTime PeriodStart, DateTime PeriodEnd) : IRequest<Result>;
