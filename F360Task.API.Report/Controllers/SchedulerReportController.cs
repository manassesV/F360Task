namespace F360Task.API.Report.Controllers;

public class SchedulerReportController : BaseController<CreateSchedullerReportCommand, ISchedulerEmailQueries, Result>
{
    public SchedulerReportController(ILogger<BaseController<CreateSchedullerReportCommand, ISchedulerEmailQueries, Result>> logger, IMediator mediator) : base(logger, mediator)
    {
    }

    public override async Task<ActionResult<Result>> Add(
        [FromHeader(Name = "x-requestid")] string requestId, 
        [FromBody] CreateSchedullerReportCommand command, 
        CancellationToken cancellationToken)
    {
        if(!Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty)
        {
            _logger.LogWarning("Invalid request ID format: {RequestId}", requestId);
            return BadRequest(Result.Fail($"Invalid request ID format: {requestId}"));
        }
        try
        {
            // Process the command
            var identifiedCommand = new IdentifiedCommand<CreateSchedullerReportCommand, Result>(command, guid);
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
