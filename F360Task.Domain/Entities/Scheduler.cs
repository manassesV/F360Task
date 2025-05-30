using F360Task.Domain.Seed;

namespace F360Task.Domain.Entities;

public abstract class Scheduler: EntityBase
{
    public SchedulerType Type { get; protected set; }
    public DateTime ScheduledAt { get; set; }
}
public enum SchedulerType
{
    Email,
    Report
}



