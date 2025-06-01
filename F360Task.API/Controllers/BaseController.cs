namespace F360Task.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public abstract class BaseController<TCommand, TService, TResult> : ControllerBase
    where TResult : class
    where TService : class
{
    protected readonly ILogger<BaseController<TCommand, TService, TResult>> _logger;
    protected readonly IMediator _mediator;

    protected BaseController(
        ILogger<BaseController<TCommand, TService, TResult>> logger,
        IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpPost]
    public abstract Task<ActionResult<TResult>> Add(
        [FromHeader(Name = "x-requestid")] string requestId,
        [FromBody] TCommand command,
        CancellationToken cancellationToken);

    [HttpGet]
    public abstract Task<ActionResult<TResult>> FindAllAsync(
        [FromServices] TService service,
        CancellationToken cancellationToken);

    [HttpGet("{id}")]
    public abstract Task<ActionResult<TResult>> FindByIdAsync(
        [FromServices] TService service,
        Guid id,
        CancellationToken cancellationToken);

    [HttpPut("{id}")]
    public abstract Task<ActionResult<TResult>> Update(
        Guid id,
        [FromBody] TCommand command,
        CancellationToken cancellationToken);

    [HttpDelete("{id}")]
    public abstract Task<ActionResult<TResult>> Delete(
        Guid id,
        CancellationToken cancellationToken);
}
