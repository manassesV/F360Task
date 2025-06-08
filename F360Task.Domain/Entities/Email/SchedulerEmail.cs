namespace F360Task.Domain.Entities.Email;

public class SchedulerEmail : Scheduler, IAggregateRoot
{
    public SchedulerEmail(string to, string subject, string body)
    {
        Type = SchedulerType.Email;
        To = to;
        Subject = subject;
        Body = body;

        AddDomainEvent(new EmailCreatedDomainEvent(to, subject, body));
    }

    public string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    
    public DateTime? LockedUntil { get; private set; }
    public DateTime? ProcessedDate { get; private set; }


}
