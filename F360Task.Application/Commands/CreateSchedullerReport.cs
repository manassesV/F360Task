namespace F360Task.Application.Commands;

public record CreateSchedullerReport<T>(string ReportType, string Format, DateTime PeriodStart, DateTime PeriodEnd):IRequest<Result>;
