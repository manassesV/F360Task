
namespace F360Task.Report.EventBusRabbitMQ;

public interface IRabbitMQConnectionProvider
{
    Task CloseAsync();
    Task<IChannel> GetChannel();
    Task InitializeAsync(CancellationToken cancellationToken);
}