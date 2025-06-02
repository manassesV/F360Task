namespace F360Task.Email.Consumer.HostingServices;

public class ConsumerEmailHostingServices : IHostedService, IDisposable
{
    private readonly IInboxMessageRepository _inboxRepository;
    private readonly ILogger<ConsumerEmailHostingServices> _logger;
    private readonly ITransactionHandler<IClientSessionHandle> _transactionHandler;

    public ConsumerEmailHostingServices(IInboxMessageRepository inboxRepository,
        ILogger<ConsumerEmailHostingServices> logger,
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
                var inboxMessages = await _inboxRepository.FindAllAsync(false, cancellationToken);

                if (inboxMessages is null || !inboxMessages.Any())
                {
                    continue;
                }

                foreach (var message in inboxMessages)
                {
                   //await  _transactionHandler.ExecuteAsync(async (data) =>
                   // {
                        message.ChangeToProcessed();
                        await _inboxRepository.UpdateAsync(message);
                        await _inboxRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
                    //});
                

                }

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
