namespace F360Task.Report.Infrastructure;

public class ApplicationDbContext : MongoDbContext, IUnitOfWork
{
    public required MongoDbSet<SchedulerReport> SchedulerReports { get; init; }
    public required MongoDbSet<ClienteRequest> ClienteRequest { get; init; }
    public required MongoDbSet<InboxMessage> InboxMessage { get; init; }
    public required MongoDbSet<OutboxMessage> OutboxMessage { get; init; }


    public ApplicationDbContext(IMongoDbConnection connection) : base(connection)
    {
        ArgumentNullException.ThrowIfNull(connection, nameof(connection));
    }
}


