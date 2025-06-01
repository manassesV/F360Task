namespace F360Task.Application.Commands;

public record CreateSchedullerEmailCommand(string to, string subject, string body) :IRequest<Result>;
