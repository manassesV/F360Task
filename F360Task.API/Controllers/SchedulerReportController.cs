

using FluentResults;

namespace F360Task.API.Controllers;

public class SchedulerReportController : BaseController<CreateSchedullerReportCommand, ISchedulerEmailQueries, Result>
{
    public SchedulerReportController(ILogger<BaseController<CreateSchedullerReportCommand, ISchedulerEmailQueries, Result>> logger, IMediator mediator) : base(logger, mediator)
    {
    }

    public override async Task<ActionResult<Result>> Add([FromBody] CreateSchedullerReportCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command);

        return Ok(result);
    }

    public override async Task<ActionResult<Result>> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteSchedulerReportCommand(id), cancellationToken);

        return Ok(result);
    }

    public override async Task<ActionResult<Result>> FindAllAsync([FromServices] ISchedulerEmailQueries service, CancellationToken cancellationToken)
    {
        var results = await service.FindAllAsync();

        return Ok(results);
    }

    public override async Task<ActionResult<Result>> FindByIdAsync([FromServices] ISchedulerEmailQueries service, Guid id, CancellationToken cancellationToken)
    {
        var result = await service.FindByIdAsync(id);

        return Ok(result);
    }

    public override async Task<ActionResult<Result>> Update(Guid id, [FromBody] CreateSchedullerReportCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateSchedulerReportCommand(id, command.ReportType, command.Format, command.PeriodStart, command.PeriodEnd), cancellationToken); ;

        return Ok(result);
    }
}
