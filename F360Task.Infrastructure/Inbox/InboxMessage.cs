namespace F360Task.Infrastructure.Inbox;
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

    public void ChangeToProcessed()
    {
        Processed = true;
        ProcessedAt = DateTime.Now;
    }
}
