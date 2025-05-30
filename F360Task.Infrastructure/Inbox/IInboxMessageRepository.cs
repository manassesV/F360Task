namespace F360Task.Infrastructure.Infrastructure.Inbox;

public interface IInboxMessageRepository
{
    Task AddAsync(InboxMessage inboxMessage);
    Task<bool> ExistAsync(string id, CancellationToken cancellationToken);
}
