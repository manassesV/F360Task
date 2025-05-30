using RabbitMQ.Client;

namespace F360Task.EventBusRabbitMQ.Publisher
{
    public class RabbitMQPublisherHostedService : IHostedService
    {

        private readonly IRabbitMqPublisher _rabbitMqPublisher;
        private readonly ILogger<RabbitMQPublisherHostedService> _logger;
      

        public RabbitMQPublisherHostedService(IRabbitMqPublisher rabbitMqPublisher,
            ILogger<RabbitMQPublisherHostedService> logger)
        {
            _rabbitMqPublisher = rabbitMqPublisher ?? throw new ArgumentException(nameof(rabbitMqPublisher));
            _logger =logger ?? throw new ArgumentException(nameof(logger));
        }


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
               await  _rabbitMqPublisher.Publish(
                    "l360",
                    "l360",
                    true,
                    null,
                    cancellationToken);
            }

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
