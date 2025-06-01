using FluentResults;

namespace F360Task.Application.Handler;

public abstract class IdentifiedCommandHandler<T, R> :
    IRequestHandler<IdentifiedCommand<T, R>, R>
    where T : IRequest<R>
{
    private readonly IMediator _mediator;
    private readonly IRequestManager _requestManager;
    private readonly ILogger<IdentifiedCommandHandler<T, R>> _logger;

    protected IdentifiedCommandHandler(
        IMediator mediator,
        IRequestManager requestManager,
        ILogger<IdentifiedCommandHandler<T, R>> logger
        )
    {
        ArgumentNullException.ThrowIfNull(logger);

        _mediator = mediator;
        _requestManager = requestManager;
        _logger = logger;
    }

    protected abstract R CreateResultForDuplicateRequest();

    public async Task<R> Handle(IdentifiedCommand<T, R> request, CancellationToken cancellationToken)
    {
        try
        {
            await _requestManager.CreateRequestForCommandAsync<T>(
            request.Id,
            cancellationToken);

            var command = request.Command;
            var commandName = command.GetGenericTypeName();
            var idProperty = string.Empty;
            var commandId = string.Empty;


            switch (command)
            {
                case CreateSchedullerEmailCommand createSchedullerEmailCommand:
                    idProperty = nameof(request.Id);
                    commandId = nameof(request.Id);
                    break;
                case CreateSchedullerReportCommand createSchedullerReportCommand:
                    idProperty = nameof(request.Id);
                    commandId = nameof(request.Id);
                    break;
                default:
                    idProperty = "Id?";
                    commandId = "n/a";
                    break;

            }

            _logger.LogInformation(
                 "Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
                 commandName,
                 idProperty,
                 commandId,
                 command);

            var result = await _mediator
                .Send(command, cancellationToken);

            _logger.LogInformation(
            "Sending result:{@Result} command - {CommandName} - {IdProperty}: {CommandId} ({@Command})",
            result,
            commandName,
            idProperty,
            commandId,
            command);


            return result;

        }
        catch (ValidationException ex)
        {
            // Process validation errors
            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray());

            _logger.LogWarning("Validation failed for command {CommandName}: {@Errors}",
                request.Command.GetGenericTypeName(),
                errors);

            var result = Result.Fail(new Error("Internal server error")
            .WithMetadata("errorDetails", ex.Message)
            .WithMetadata("exceptionType", ex.GetType().Name));


            return (R)Convert.ChangeType(result, typeof(R));

        }
        catch (RequestException ex)
        {
            _logger.LogError(ex, "Request failed for accord GUID: {AccordGuid}. Error: {ErrorMessage}",
                request.Id, ex.Message);

            var result = Result.Fail(new Error("Internal server error")
                .WithMetadata("errorDetails", ex.Message)
                .WithMetadata("exceptionType", ex.GetType().Name)
                .WithMetadata("stackTrace", ex.StackTrace)); // Optional: include stack trace

            return (R)Convert.ChangeType(result, typeof(R));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error processing command {CommandName}",
                request.Command.GetGenericTypeName());

            var result = Result.Fail(new Error("Internal server error")
            .WithMetadata("errorDetails", ex.Message)
            .WithMetadata("exceptionType", ex.GetType().Name));

            return (R)Convert.ChangeType(result, typeof(R));
        }
    }
}
