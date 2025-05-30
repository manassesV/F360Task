namespace F360Task.Domain.Events;

public record EmailCreatedDomainEvent(
     string To,
     string Subject,
     string Body
    ):INotification;
