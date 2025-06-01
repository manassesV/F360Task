namespace F360Task.Application.Commands;

public record CreateSchedullerReportCommand(string ReportType, string Format, DateTime PeriodStart, DateTime PeriodEnd):IRequest<Result>;
