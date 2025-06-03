namespace F360Task.EventBusRabbitMQ;

public class RabbitMQConnectionProvider : IRabbitMQConnectionProvider
{
    private IConnection? _connection;
    private IChannel _channelPublish;
    private IChannel _channelConsumer;
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

    public async Task<IChannel> GetChannelPublish()
    {
        if (!_initialized || _connection == null)
            throw new InvalidOperationException("Connection not initialized yet.");

        if(_channelPublish != null && !_channelPublish.IsOpen)
        {
            await _channelPublish.CloseAsync();
            _channelPublish.Dispose();
            _channelPublish = await _connection.CreateChannelAsync();
        }

        if(_channelPublish == null) 
            _channelPublish = await _connection.CreateChannelAsync();

        return _channelPublish;
    }

    public async Task<IChannel> GetChannelConsumer()
    {
        if (!_initialized || _connection == null)
            throw new InvalidOperationException("Connection not initialized yet.");

        if (_channelConsumer != null && !_channelConsumer.IsOpen)
        {
            await _channelConsumer.CloseAsync();
            _channelConsumer.Dispose();
            _channelConsumer = await _connection.CreateChannelAsync();
        }

        if (_channelConsumer == null)
            _channelConsumer = await _connection.CreateChannelAsync();

        return _channelConsumer;
    }
}
