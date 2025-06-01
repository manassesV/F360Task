namespace F360Task.Application.Validations;

public class IdentifierCommandValidator<T>:AbstractValidator<IdentifiedCommand<T, Result>>
    where T : IRequest<Result>
{
    public IdentifierCommandValidator()
    {
       // RuleFor(command => command.Id).NotEmpty();
    }
}
