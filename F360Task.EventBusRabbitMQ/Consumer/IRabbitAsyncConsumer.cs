
namespace F360Task.EventBusRabbitMQ.Consumer
{
    public interface IRabbitAsyncConsumer : IAsyncBasicConsumer
    {
        void SetChannel(IChannel channel);
    }
}