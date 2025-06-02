namespace F360Task.Report.Domain.Seed;

public abstract class EntityBase
{
    public Guid Id { get; protected set; }

    protected EntityBase()
    {
        Id = Guid.NewGuid(); // Ou deixe o domínio gerar
    }

    private List<INotification> _domainEvents;
    public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(INotification domainEvent)
    {
        _domainEvents  = _domainEvents ?? new List<INotification>();
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(INotification domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvent()
    {
        _domainEvents.Clear();
    }

    public override bool Equals(object? obj)
    {
        if (obj is not EntityBase other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        return Id == other.Id;
    }

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(EntityBase a, EntityBase b)
    {
        if (a is null && b is null) return true;
        if (a is null || b is null) return false;

        return a.Equals(b);
    }

    public static bool operator !=(EntityBase a, EntityBase b) => !(a == b);
}
