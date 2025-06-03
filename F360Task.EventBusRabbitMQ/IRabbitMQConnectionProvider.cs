
namespace F360Task.EventBusRabbitMQ
{
    public interface IRabbitMQConnectionProvider
    {
        Task CloseAsync();
        Task<IChannel> GetChannelPublish();
        Task<IChannel> GetChannelConsumer();
        Task InitializeAsync(CancellationToken cancellationToken);
    }
}