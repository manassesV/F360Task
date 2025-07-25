﻿namespace F360Task.Infrastructure;

public class EmailDbContext : MongoDbContext, IUnitOfWork
{
    public required MongoDbSet<SchedulerEmail> SchedulerEmails { get; init; }
    public required MongoDbSet<ClienteRequest> ClienteRequest { get; init; }
    public required MongoDbSet<InboxMessage> InboxMessage { get; init; }
    public required MongoDbSet<OutboxMessage> OutboxMessage { get; init; }


    public EmailDbContext(IMongoDbConnection connection) : base(connection)
    {
        ArgumentNullException.ThrowIfNull(connection, nameof(connection));
    }
}


