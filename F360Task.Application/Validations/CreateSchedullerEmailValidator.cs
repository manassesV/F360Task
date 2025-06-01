namespace F360Task.Application.Validations;


public class CreateSchedulerEmailValidator : AbstractValidator<CreateSchedullerEmailCommand>
{
    public CreateSchedulerEmailValidator()
    {
        RuleFor(command => command.to)
            .NotEmpty().WithMessage("Recipient email is required.")
            .EmailAddress().WithMessage("Invalid email format.")
            .MaximumLength(255).WithMessage("Email is too long.");

        RuleFor(command => command.subject)
            .NotEmpty().WithMessage("Subject is required.")
            .MaximumLength(200).WithMessage("Subject is too long.");

        RuleFor(command => command.body)
            .NotEmpty().WithMessage("Email body is required.");
    }
}
