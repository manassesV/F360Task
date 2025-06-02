namespace F360Task.Report.Application.Validations;

public class CreateSchedullerReportValidator:AbstractValidator<CreateSchedullerReportCommand>
{
    public CreateSchedullerReportValidator()
    {
        RuleFor(x => x.ReportType)
          .NotEmpty().WithMessage("Report type is required.")
          .MaximumLength(100).WithMessage("Report type is too long.");

        RuleFor(x => x.Format)
            .NotEmpty().WithMessage("Format is required.")
            .MaximumLength(50).WithMessage("Format is too long.");

        RuleFor(x => x.PeriodStart)
            .LessThan(x => x.PeriodEnd)
            .WithMessage("Period start must be before period end.");

        RuleFor(x => x.PeriodEnd)
            .GreaterThan(x => x.PeriodStart)
            .WithMessage("Period end must be after period start.");

    }
}
