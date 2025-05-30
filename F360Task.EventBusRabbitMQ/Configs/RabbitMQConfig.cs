namespace F360Task.EventBusRabbitMQ.Configs;

public class RabbitMQConfig
{
    public string HostName { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string VirtualHost { get; set; }
    public bool UseSsl { get; set; }
    public int Port { get; set; }
}
