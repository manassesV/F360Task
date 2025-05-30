
namespace F360Task.EventBusRabbitMQ
{
    public interface IRabbitMQConnectionProvider
    {
        Task CloseAsync();
        IConnection GetConnection();
        Task InitializeAsync(CancellationToken cancellationToken);
    }
}