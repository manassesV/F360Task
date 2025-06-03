
namespace F360Task.EventBusRabbitMQ
{
    public interface IRabbitMQConnectionProvider
    {
        Task CloseAsync();
        Task<IChannel> GetChannel();
        Task InitializeAsync(CancellationToken cancellationToken);
    }
}