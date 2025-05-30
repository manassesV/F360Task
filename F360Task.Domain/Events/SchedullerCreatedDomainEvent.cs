namespace F360Task.Domain.Events;

public record SchedullerCreatedDomainEvent(
      string ReportType, 
      string Format,
      DateTime PeriodStart,
      DateTime PeriodEnd):INotification;
