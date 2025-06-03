namespace F360Task.Report.EventBusRabbitMQ;

public class RabbitMQConnectionProvider : IRabbitMQConnectionProvider
{
    private IConnection? _connection;
    private IChannel _channel;
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

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        if (_initialized) return;

        await _initLock.WaitAsync();

        try
        {
            if (_initialized) return;

            Func<Task<IConnection>> factory = () =>
                Task.Run(() => _factory.CreateConnectionAsync());

            _connection = await _retry.ExecuteAsync(factory, cancellationToken);

            _initialized = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[RabbitMQ Init Error] {ex.Message}");
            throw; // Important: let it propagate so the app can restart or retry startup
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

    public async Task<IChannel> GetChannel()
    {
        if (!_initialized || _connection == null)
            throw new InvalidOperationException("Connection not initialized yet.");

        if(_channel != null && !_channel.IsOpen)
        {
            await _channel.CloseAsync();
            _channel.Dispose();
            _channel = await _connection.CreateChannelAsync();
        }

        if(_channel == null) 
            _channel = await _connection.CreateChannelAsync();

        return _channel;
    }
}
