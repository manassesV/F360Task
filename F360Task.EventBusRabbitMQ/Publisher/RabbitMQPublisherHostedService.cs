using Amazon.Runtime;
using F360Task.Infrastructure.Outbox;
using RabbitMQ.Client;

namespace F360Task.EventBusRabbitMQ.Publisher
{
    public class RabbitMQPublisherHostedService : IHostedService
    {

        private readonly IRabbitMqPublisher _rabbitMqPublisher;
        private readonly ILogger<RabbitMQPublisherHostedService> _logger;
        private readonly IOutboxMessageRepository _outboxMessageRepository;
        private readonly ITransactionHandler<IClientSessionHandle> _transactionHandler;

        public RabbitMQPublisherHostedService(IRabbitMqPublisher rabbitMqPublisher,
            ILogger<RabbitMQPublisherHostedService> logger,
            IOutboxMessageRepository outboxMessageRepository,
            ITransactionHandler<IClientSessionHandle> transactionHandler)
        {
            _rabbitMqPublisher = rabbitMqPublisher ?? throw new ArgumentException(nameof(rabbitMqPublisher));
            _logger = logger ?? throw new ArgumentException(nameof(logger));
            _outboxMessageRepository = outboxMessageRepository ?? throw new ArgumentException(nameof(outboxMessageRepository));
            _transactionHandler = transactionHandler;
        }


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            const int pollingIntervalMs = 300000;
            var failledAttemps = 0;
            const int maxFailedAttempts = 5;



            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {

                    await Task.Delay(pollingIntervalMs, cancellationToken);


                    var outboxMessages =
                        await _outboxMessageRepository
                        .FindAllAsync(false, cancellationToken);

                    if (outboxMessages.Count == 0)
                    {
                        failledAttemps = 0;
                        continue;
                    }

                    var options = new ParallelOptions
                    {
                        CancellationToken = cancellationToken,
                        MaxDegreeOfParallelism = Environment.ProcessorCount
                    };

                    await Parallel.ForEachAsync(outboxMessages, options, async (message, ct) =>
                    {
                        try
                        {
                            if (message.Payload is null)
                            {
                                _logger.LogWarning("Skipping null message or payload");

                                return;
                            }

                            byte[] temp = Encoding.UTF8.GetBytes(message.Payload);

                            await _rabbitMqPublisher.Publish(
                              "l360",
                              "l360",
                              true,
                              message.Id.ToString(),
                              temp,
                              cancellationToken);

                            await _transactionHandler.ExecuteAsync(() =>
                            {
                                message.ChangeToProcessed();
                                _outboxMessageRepository.u
                            });
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                     

                    });
                    
                }
                catch (Exception)
                {
                   
                }
             
            }

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

public static class Project
{
    public static ReadOnlyMemory<byte> AsReadOnlyMemory(this byte[] bytes)
    => bytes.AsMemory();
}