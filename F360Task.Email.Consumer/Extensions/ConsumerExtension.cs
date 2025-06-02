namespace F360Task.Email.Consumer.Extensions;

public static class ConsumerExtension
{
    public static IHostApplicationBuilder AddConsumer(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        builder.AddInfrastructure();

        services.Configure<EmailWorkerOptions>(
            builder.Configuration.GetSection("EmailWorker"));

        services.AddScoped<ITransactionHandler<IClientSessionHandle>, TransactionHandler>();

        services.AddHostedService<ConsumerEmailHostingServices>();

        return builder;
    }
}
