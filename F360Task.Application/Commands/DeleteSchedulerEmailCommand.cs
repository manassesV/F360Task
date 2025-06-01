namespace F360Task.Application.Commands;

public record DeleteSchedulerEmailCommand(Guid Id) : IRequest<Result>;