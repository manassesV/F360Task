namespace F360Task.Application.Commands;

public record CreateSchedullerReport(string ReportType, string Format, DateTime PeriodStart, DateTime PeriodEnd):IRequest<Result>;
