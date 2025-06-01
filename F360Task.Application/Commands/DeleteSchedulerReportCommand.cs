namespace F360Task.Application.Commands;


public record DeleteSchedulerReportCommand(Guid Id) : IRequest<Result>;
  
