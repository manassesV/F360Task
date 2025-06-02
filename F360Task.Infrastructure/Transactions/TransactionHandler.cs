namespace F360Task.Infrastructure.Transactions;


public class TransactionHandler : ITransactionHandler<IClientSessionHandle>
{
    private readonly IMongoDbConnection _connection;
    public TransactionHandler(IMongoDbConnection connection)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
    }

    public async Task<bool> ExecuteAsync(Func<IClientSessionHandle, Task> action, CancellationToken cancellationToken = default)
    {
        using var session = await _connection.Client.StartSessionAsync(cancellationToken: cancellationToken);

        session.StartTransaction();

        try
        {
            await action(session);
            await session.CommitTransactionAsync(cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            await session.AbortTransactionAsync(cancellationToken);
            throw new Exception("Transaction failed", ex);
        }

    }
}
