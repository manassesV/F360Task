namespace F360Task.Application.Commands;

public class UpdateSchedulerEmailCommand(Guid id,string to, string subject, string body) : IRequest<Result>;