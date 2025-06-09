namespace F360Task.Report.Consumer.Extensions;

public class ConsumerReportHostingServices : IHostedService, IDisposable
{
    private readonly IInboxMessageRepository _inboxRepository;
    private readonly ILogger<ConsumerReportHostingServices> _logger;
    private readonly ITransactionHandler<IClientSessionHandle> _transactionHandler;

    public ConsumerReportHostingServices(IInboxMessageRepository inboxRepository,
        ILogger<ConsumerReportHostingServices> logger,
        ITransactionHandler<IClientSessionHandle> transactionHandler)
    {
        _inboxRepository = inboxRepository;
        _logger = logger;
        _transactionHandler = transactionHandler;
    }

    public void Dispose() => _inboxRepository.UnitOfWork.Dispose();


    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(30));

        try
        {
            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                var now = DateTime.UtcNow;
                var lockDuration = TimeSpan.FromSeconds(30);

                var inboxMessages = await _inboxRepository.FindAllAsync(false,
                    now,
                    lockDuration,
                    cancellationToken);

                if (inboxMessages is null || !inboxMessages.Any())
                {
                    continue;
                }

                foreach (var message in inboxMessages)
                {
                    await _transactionHandler.ExecuteAsync(async (data) =>
                     {
                         message.ChangeToProcessed();
                         await _inboxRepository.UpdateAsync(message);
                         await _inboxRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
                     });


                }

                _logger.LogInformation("🟢 {Count} mensagens processadas em {Time}", inboxMessages.Count(), DateTimeOffset.Now);

            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("🛑 ReportWorker foi cancelado.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro inesperado no ReportWorker");
        }
        finally
        {
            _logger.LogInformation("✅ ReportWorker finalizado.");
        }

    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
