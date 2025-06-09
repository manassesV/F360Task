namespace F360Task.Report.Infrastructure.Inbox;
public class InboxMessage
{
    public InboxMessage(string id, string payload)
    {
        Id = id;
        Payload = payload;
        ReceivedAt = DateTime.Now;
    }

    public string Id { get; private set; }
    public string Payload { get; private set; }
    public bool Processed { get; private set; }
    public DateTime ReceivedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public DateTime? LockedUntil { get; private set; }

    public void ChangeToProcessed()
    {
        Processed = true;
        ProcessedAt = DateTime.Now;
    }

    public void ChangeToLocked(DateTime now, TimeSpan lockDuration)
    {
        LockedUntil = now.Add(lockDuration);
    }
}
