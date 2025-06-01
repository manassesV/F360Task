namespace F360Task.Application.Handler;

public class CreateSchedullerReportHandler : IRequestHandler<CreateSchedullerReportCommand, Result>
{
    private readonly ISchedulerReportRepository _schedulerReportRepository;
    private readonly IOutboxMessageRepository _outboxMessageRepository;
    private readonly ITransactionHandler<IClientSessionHandle> _transactionHandler;
    private readonly ILogger<CreateSchedullerReportHandler> _logger;

    public CreateSchedullerReportHandler(ISchedulerReportRepository schedulerReportRepository,
        IOutboxMessageRepository outboxMessageRepository,
        ITransactionHandler<IClientSessionHandle> transactionHandler,
        ILogger<CreateSchedullerReportHandler> logger)
    {
        _schedulerReportRepository = schedulerReportRepository ?? throw new ArgumentNullException(nameof(schedulerReportRepository));
        _outboxMessageRepository = outboxMessageRepository ?? throw new ArgumentNullException(nameof(outboxMessageRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _transactionHandler = transactionHandler ?? throw new ArgumentNullException(nameof(transactionHandler));
    }
    public async Task<Result> Handle(CreateSchedullerReportCommand createSchedullerReport, CancellationToken cancellationToken)
    {
        try
        {
            if(createSchedullerReport is null)
                return Result.Fail("Request cannot be null");
            

            var schedulerReport = new SchedulerReport(createSchedullerReport.ReportType,
                createSchedullerReport.Format,
                createSchedullerReport.PeriodStart,
                createSchedullerReport.PeriodEnd);

            await _transactionHandler.ExecuteAsync(async (session) =>
            {
                await _schedulerReportRepository.AddAsync(schedulerReport);

                var outboxPattern = new OutboxMessage("Report", nameof(CreateSchedullerReportCommand), JsonSerializer.Serialize(schedulerReport));
                await _outboxMessageRepository.AddAsync(outboxPattern);

                await _schedulerReportRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            });

            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create scheduled report for {Type}", createSchedullerReport.ReportType);
            return Result.Fail($"Failed to save job report: {ex.Message}");
        }
    }
}
