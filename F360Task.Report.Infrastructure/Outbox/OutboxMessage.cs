namespace F360Task.Report.Infrastructure.Outbox;

public class OutboxMessage
{
    public OutboxMessage(string exchange, string queue, string payload)
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.Now;
        Exchange = exchange;
        Payload = payload;
        Processed = false;
        Queue = queue;
    }

    public Guid Id { get; private set; } 
    public DateTime CreatedAt { get; private set; }
    public string Exchange { get; private set; }
    public string Queue { get; private set; }
    public string Payload { get; private set; }
    public bool Processed { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public int Retry { get; private set; }

    public void ChangeToProcessed()
    {
        Processed = true;
        ProcessedAt = DateTime.Now;
    }

    public void ChangeToFailed()
    {
        Processed = false;
        ProcessedAt = null;
    }

    public void IncrementRetryCount()
    {
        Retry++;
    }


}
