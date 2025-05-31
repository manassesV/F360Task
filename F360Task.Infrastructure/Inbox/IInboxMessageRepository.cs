namespace F360Task.Infrastructure.Inbox;

public interface IInboxMessageRepository
{
    IUnitOfWork UnitOfWork { get; }

    Task AddAsync(InboxMessage inboxMessage);
    Task<bool> ExistAsync(string id, CancellationToken cancellationToken);

    Task<List<InboxMessage>> FindAllAsync(bool processed,
       CancellationToken cancellationToken);
}
