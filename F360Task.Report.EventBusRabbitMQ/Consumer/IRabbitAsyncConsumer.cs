
namespace F360Task.Report.EventBusRabbitMQ.Consumer;

public interface IRabbitAsyncConsumer : IAsyncBasicConsumer
{
    void SetChannel(IChannel channel);
}