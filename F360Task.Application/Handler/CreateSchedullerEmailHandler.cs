﻿using F360Task.Domain.Entities.Email;
using Microsoft.Extensions.Options;

namespace F360Task.Application.Handler;

public class CreateSchedullerEmailHandler : IRequestHandler<CreateSchedullerEmailCommand, Result>
{
    private readonly ISchedulerEmailRepository _schedulerEmailRepository;
    private readonly IOutboxMessageRepository _outboxMessageRepository;
    private readonly ITransactionHandler<IClientSessionHandle> _transactionHandler;
    private readonly ILogger<CreateSchedullerEmailHandler> _logger;
    

    public CreateSchedullerEmailHandler(ISchedulerEmailRepository schedulerEmail,
        IOutboxMessageRepository outboxMessageRepository,
        ITransactionHandler<IClientSessionHandle> transactionHandler,
        ILogger<CreateSchedullerEmailHandler> logger)
    {
        _schedulerEmailRepository = schedulerEmail ?? throw new ArgumentNullException(nameof(schedulerEmail));
        _outboxMessageRepository = outboxMessageRepository ?? throw new ArgumentNullException(nameof(outboxMessageRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _transactionHandler = transactionHandler ?? throw new ArgumentNullException(nameof(transactionHandler));
    }
    public async Task<Result> Handle(CreateSchedullerEmailCommand createSchedullerEmail, CancellationToken cancellationToken)
    {
        try
        {
            if(createSchedullerEmail is null)
                return Result.Fail("Request cannot be null");
            

            var schedulerEmail = new SchedulerEmail(createSchedullerEmail.to, createSchedullerEmail.subject, createSchedullerEmail.body);

            await _transactionHandler.ExecuteAsync(async (session) =>
            {
                await _schedulerEmailRepository.AddAsync(schedulerEmail);
                await _schedulerEmailRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

                var outboxPattern = new OutboxMessage("Email", "EnviarEmail", JsonSerializer.Serialize(schedulerEmail));
                await _outboxMessageRepository.AddAsync(outboxPattern);
                await _outboxMessageRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            });

            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create scheduled email for {subject}", createSchedullerEmail.subject);
            return Result.Fail($"Failed to save job email: {ex.Message}");
        }
    }
}


//Use for Idempotency in command process
public class CreateSchedullerEmailIdentifiedCommandHandler : IdentifiedCommandHandler<CreateSchedullerEmailCommand, Result>
{
    public CreateSchedullerEmailIdentifiedCommandHandler(
        IMediator mediator,
        IRequestManager requestManager,
        ILogger<IdentifiedCommandHandler<CreateSchedullerEmailCommand, Result>> logger
        ) : base(mediator, requestManager, logger)
    {
    }

    protected override Result CreateResultForDuplicateRequest()
    {
        return Result.Fail("deplicated request");
    }
}