namespace F360Task.Report.Application.Commands;


public record DeleteSchedulerReportCommand(Guid Id) : IRequest<Result>;
  
