namespace F360Task.Infrastructure.Outbox;

public class OutboxMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string Type { get; set; }
    public string Payload { get; set; }
    public bool Processed { get; set; }
    public DateTime? ProcessedAt { get; set; }

}
