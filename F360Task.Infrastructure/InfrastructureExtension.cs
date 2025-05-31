using F360Task.Infrastructure.Inbox;
using MongoDB.Driver.Linq;
using MongoFramework.Infrastructure.Diagnostics;

namespace F360Task.Infrastructure;

public static class InfrastructureExtension
{

    public static IHostApplicationBuilder AddInfrastructure(this IHostApplicationBuilder builder)
    {
        var services = builder.Services; 
        // Register MongoDB context and repositories here
        services.AddMongoDbContext(builder.Configuration);
        services.AddRepositories();

        return builder;
    }

    public static IServiceCollection AddMongoDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MongoDb");

        services.AddSingleton<IMongoClient>(provider =>
        {
            var url = new MongoUrl(connectionString);
            var settings = MongoClientSettings.FromUrl(url);
            settings.LinqProvider = LinqProvider.V2;
            return new MongoClient(settings);
        });

        services.AddScoped<IMongoDbConnection>(provider =>
        {
            var client = provider.GetRequiredService<IMongoClient>();
            var url = new MongoUrl(connectionString);
            return new MongoDbConnection(client, url);
        });

        services.AddScoped<ApplicationDbContext>();
        services.AddScoped<ITransactionHandler<IClientSessionHandle>, TransactionHandler>();


        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IInboxMessageRepository, InboxMessageRepository>();
        services.AddScoped<IOutboxMessageRepository, OutboxMessageRepository>();
        services.AddScoped<ISchedulerEmailRepository, SchedulerEmailRepository>();
        services.AddScoped<ISchedulerReportRepository, SchedulerReportRepository>();

        return services;
    }
}
public class MongoDbConnection : IMongoDbConnection
{
    private readonly IMongoClient _client;
    private readonly MongoUrl _url;

    public MongoUrl Url => _url;
    public IMongoClient Client => _client;
    public IDiagnosticListener DiagnosticListener { get; set; } = new NoOpDiagnosticListener();

    public MongoDbConnection(IMongoClient client, MongoUrl url)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _url = url ?? throw new ArgumentNullException(nameof(url));
    }

    public IMongoDatabase GetDatabase() => _client.GetDatabase(_url.DatabaseName);

    // No disposal needed since client is managed externally
    public void Dispose() { }

    public static MongoDbConnection FromConnectionString(string connectionString, Action<MongoClientSettings> configureSettings = null)
    {
        var url = new MongoUrl(connectionString);
        var settings = MongoClientSettings.FromUrl(url);
        configureSettings?.Invoke(settings);
        settings.LinqProvider = LinqProvider.V2;
        var client = new MongoClient(settings);
        return new MongoDbConnection(client, url);
    }
}