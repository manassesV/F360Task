namespace F360Task.Infrastructure.Inbox;
public class InboxMessage
{
    public InboxMessage(string id, string source)
    {
        Id = id;
        Source = source;
        ReceivedAt = DateTime.Now;
    }

    public string Id { get; set; }
    public string Source { get; set; }
    public bool Processed { get; set; }
    public DateTime ReceivedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }

    public void ChangeToProcessed()
    {
        Processed = true;
        ProcessedAt = DateTime.Now;
    }
}
