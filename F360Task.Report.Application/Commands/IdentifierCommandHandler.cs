namespace F360Task.Report.Application.Commands;

public class IdentifierCommandHandler<T, R> : IRequestHandler<IdentifiedCommand<T, R>, R>
    where T : IRequest<R>
{

    private readonly IMediator _mediator;
    private readonly ILogger<IdentifierCommandHandler<T, R>> _logger;
    private readonly IRequestManager _requestManager;

    public IdentifierCommandHandler(
        IMediator mediator, 
        ILogger<IdentifierCommandHandler<T, R>> logger,
        IRequestManager requestManager)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _requestManager = requestManager;
    }

    protected virtual R CreateResultForDuplicateRequest()
    {
        return default(R);
    }


    public async Task<R> Handle(IdentifiedCommand<T, R> request, CancellationToken cancellationToken)
    {
        try
        {
            await _requestManager.CreateRequestForCommandAsync<T>(request.Id, cancellationToken);

        }
        catch (RequestException ex)
        {
            _logger.LogWarning(ex, "Duplicate request detected for {RequestId}", request.Id);

            return CreateResultForDuplicateRequest();
        }

        _logger.LogInformation("Processing request {RequestName} with identifier {Identifier}.",
            request.GetType().Name, request.GetHashCode());

        return await _mediator.Send(request, cancellationToken);
    }
}
