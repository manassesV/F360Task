namespace F360Task.Report.Infrastructure.Infrastructure.Contexts.Idempotent;

public class ClienteRequest
{
    public Guid Id { get; set; }
    public string? Name { get; set; } 
    public DateTime DateTime { get; set; } 

}
