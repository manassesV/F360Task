using Polly.Retry;

namespace F360Task.EventBusRabbitMQ;

public class RabbitMQConnectionProvider : IRabbitMQConnectionProvider
{
    private IConnection? _connection;
    private readonly IConnectionFactory _factory;
    private readonly IRetryResiliency _retry;
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private bool _initialized = false;
    public RabbitMQConnectionProvider(IConnectionFactory factory,
        IRetryResiliency retry)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory)); ;
        _retry = retry ?? throw new ArgumentNullException(nameof(retry)); ;
    }

    public async Task InitializeAsync()
    {
        if (_initialized) return;

        await _initLock.WaitAsync();

        try
        {
            if (_initialized) return;

            Func<Task<IConnection>> factory = () => _factory.CreateConnectionAsync();
            _connection = await _retry.ExecuteAsync<IConnection>(factory);

            _initialized = true;
        }
        finally
        {
            _initLock.Release();
        }


    }

    public async Task CloseAsync()
    {
        if (_connection is null)
            return;

        await _connection.CloseAsync();
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }

    public IConnection GetConnection()
    {
        if (!_initialized || _connection == null)
            throw new InvalidOperationException("Connection not initialized yet.");

        return _connection;
    }
}
