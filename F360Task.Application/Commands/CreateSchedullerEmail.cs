namespace F360Task.Application.Commands;

public record CreateSchedullerEmail(string to, string subject, string body) :IRequest<Result>;
