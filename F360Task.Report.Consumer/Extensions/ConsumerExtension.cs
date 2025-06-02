namespace F360Task.Report.Consumer.Extensions;

public static class ConsumerExtension
{
    public static IHostApplicationBuilder AddConsumer(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        builder.AddInfrastructure();

        services.Configure<ReportWorkerOptions>(
            builder.Configuration.GetSection("ReportWorker"));

        services.AddScoped<ITransactionHandler<IClientSessionHandle>, TransactionHandler>();

        services.AddHostedService<ConsumerReportHostingServices>();

        return builder;
    }
}
