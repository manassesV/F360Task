using F360Task.Application.Commands;

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
        [FromHeader(Name = "x-requestid")] string requestId,
        [FromBody] CreateSchedullerEmailCommand command,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty)
        {
            _logger.LogWarning("Invalid request ID format: {RequestId}", requestId);
            return BadRequest(Result.Fail($"Invalid request ID format: {requestId}"));
        }
        try
        {
            // Process the command
            var identifiedCommand = new IdentifiedCommand<CreateSchedullerEmailCommand, Result>(command, guid);
            var result = await _mediator.Send(identifiedCommand, cancellationToken);

            if (result.IsSuccess)
            {
                _logger.LogInformation("Successfully processed request {RequestId}", requestId);
                return Ok(result);
            }

            _logger.LogWarning("Failed to process request {RequestId}: {Error}", requestId, result.Errors);
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing request {RequestId}", requestId);
            return StatusCode(500, Result.Fail("An unexpected error occurred"));
        }
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
