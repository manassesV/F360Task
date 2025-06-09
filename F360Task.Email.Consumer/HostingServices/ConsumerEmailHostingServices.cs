namespace F360Task.Email.Consumer.HostingServices;

public class ConsumerEmailHostingServices : IHostedService, IDisposable
{
    private readonly IInboxMessageRepository _inboxRepository;
    private readonly ILogger<ConsumerEmailHostingServices> _logger;

    public ConsumerEmailHostingServices(IInboxMessageRepository inboxRepository,
        ILogger<ConsumerEmailHostingServices> logger,
        ITransactionHandler<IClientSessionHandle> transactionHandler)
    {
        _inboxRepository = inboxRepository;
        _logger = logger;
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

                await Parallel.ForEachAsync(inboxMessages, new ParallelOptions
                {
                    CancellationToken = cancellationToken,
                    MaxDegreeOfParallelism = Environment.ProcessorCount
                }, async (message, ct) =>
                {
                   
                         message.ChangeToProcessed();
                         await _inboxRepository.UpdateAsync(message);                     

                });

                _logger.LogInformation("🟢 {Count} mensagens processadas em {Time}", inboxMessages.Count(), DateTimeOffset.Now);

            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("🛑 EmailWorker foi cancelado.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro inesperado no EmailWorker");
        }
        finally
        {
            _logger.LogInformation("✅ EmailWorker finalizado.");
        }

    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
