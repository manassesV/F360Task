namespace F360Task.Report.Infrastructure.Infrastructure.Contexts.Idempotent;
public class RequestManager : IRequestManager
{
    private readonly ILogger<RequestManager> _logger;
    private readonly ITransactionHandler<IClientSessionHandle> _transactionHandler;
    private readonly IClienteRequestRepository _clienteRequestRepository;

    public RequestManager(ApplicationDbContext context,
        ILogger<RequestManager> logger,
        ITransactionHandler<IClientSessionHandle> transactionHandler,
        IClienteRequestRepository clienteRequestRepository)
    {
        _logger = logger;
        _transactionHandler = transactionHandler;
        _clienteRequestRepository = clienteRequestRepository;
    }

    public async Task CreateRequestForCommandAsync<T>(Guid id, CancellationToken cancellationToken)
    {
        var exist = await _clienteRequestRepository.ExistAsync(id, cancellationToken);

        await _transactionHandler.ExecuteAsync(async session =>
        {
            try
            {

                var request = exist ?
                    throw new RequestException($"Request with {id} already exists") :
                    new ClienteRequest
                    {
                        Id = id,
                        DateTime = DateTime.UtcNow,
                        Name = typeof(T).Name
                    };

               await _clienteRequestRepository.AddAsync(request);
            } 
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating request for command {CommandName} with ID {Id}.", typeof(T).Name, id);
                throw; 
            }
        }, cancellationToken);
     
    }

 
}
