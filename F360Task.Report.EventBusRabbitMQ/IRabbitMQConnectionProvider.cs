
namespace F360Task.Report.EventBusRabbitMQ;

public interface IRabbitMQConnectionProvider
{
    Task CloseAsync();
    Task<IChannel> GetPublishChannel();
    Task<IChannel> GetConsumerChannel();
    Task InitializeAsync(CancellationToken cancellationToken);
}