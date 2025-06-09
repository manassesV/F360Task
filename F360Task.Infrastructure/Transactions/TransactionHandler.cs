namespace F360Task.Infrastructure.Transactions;


public class TransactionHandler : ITransactionHandler<IClientSessionHandle>
{
    private readonly IMongoDbConnection _connection;
    private readonly ILogger<TransactionHandler> _logger;
    public TransactionHandler(IMongoDbConnection connection, ILogger<TransactionHandler> logger)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task ExecuteAsync(Func<IClientSessionHandle, Task> action, CancellationToken cancellationToken = default)
    {
        using var session = await _connection.Client.StartSessionAsync(cancellationToken: cancellationToken);
        session.StartTransaction();

        try
        {
            await action(session);

           
           
            // Add debug logging before commit
            _logger.LogDebug("Committing transaction for session {SessionId}", session.ServerSession.Id);

            // Explicit commit with timeout for Docker environments
            await session.CommitTransactionAsync(
                cancellationToken: new CancellationTokenSource(TimeSpan.FromSeconds(30)).Token);

            _logger.LogDebug("Transaction committed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed. Attempting rollback...");

            try
            {
                await session.AbortTransactionAsync(CancellationToken.None);
                _logger.LogInformation("Transaction rolled back successfully");
            }
            catch (Exception rollbackEx)
            {
                _logger.LogCritical(rollbackEx, "Failed to abort transaction!");
            }

            throw new Exception("Transaction operation failed", ex);
        }
    }
}
