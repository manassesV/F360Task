namespace F360Task.API.Controllers;

public class SchedulerEmailController
    : BaseController<CreateSchedullerEmailCommand, ISchedulerEmailQueries, Result>
{
    public SchedulerEmailController(
        ILogger<BaseController<CreateSchedullerEmailCommand, ISchedulerEmailQueries, Result>> logger,
        IMediator mediator)
        : base(logger, mediator)
    {
    }

    [HttpPost]
    public override async Task<ActionResult<Result>> Add(
        [FromBody] CreateSchedullerEmailCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public override async Task<ActionResult<Result>> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteSchedulerEmailCommand(id), cancellationToken);

        return Ok(result);
    }

    [HttpGet]
    public override async Task<ActionResult<Result>> FindAllAsync(
        [FromServices] ISchedulerEmailQueries service,
        CancellationToken cancellationToken)
    {
        var result = await service.FindAllAsync();
        return Ok(result);
    }

    public override async Task<ActionResult<Result>> FindByIdAsync([FromServices] ISchedulerEmailQueries service, Guid id, CancellationToken cancellationToken)
    {
        var result = await service.FindByIdAsync(id);
        return Ok(result);
    }


    public override async Task<ActionResult<Result>> Update(Guid id, [FromBody] CreateSchedullerEmailCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateSchedulerEmailCommand(id, command.to, command.subject, command.body), cancellationToken);;

        return Ok(result);
    }
}
