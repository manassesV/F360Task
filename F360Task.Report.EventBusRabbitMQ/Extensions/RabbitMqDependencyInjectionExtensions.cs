namespace F360Task.Report.EventBusRabbitMQ.Extensions;

public static class RabbitMqDependencyInjectionExtensions
{
    //public static IEventBusBuilder AddRabbitMqEventBus(this IEventBusBuilder builder,
    //    Action<EventBusOptions> configureOptions,
    //    IConfiguration configuration)
    //{
    //    if (builder is null)
    //    {
    //        throw new ArgumentNullException(nameof(builder));
    //    }
    //    if (configureOptions is null)
    //    {
    //        throw new ArgumentNullException(nameof(configureOptions));
    //    }
    //    var options = new EventBusOptions();
    //    configureOptions(options);
    //   // builder.Services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp => new EventBusRabbitMQ(sp, options));
    //    //builder.Services.AddSingleton<IIntegrationEventHandler, IntegrationEventHandler>();

       
       





    //    return builder;
    //}
    public static IHostApplicationBuilder AddRabbitMqEventBus(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;
        builder.Services.Configure<RabbitMQConfig>(builder.Configuration.GetSection("RabbitMQ"));

        var rabbitConfig = builder.Configuration
            .GetSection("RabbitMQ")
            .Get<RabbitMQConfig>();

        services.AddSingleton<IConnectionFactory>(sp => new ConnectionFactory
        {
            HostName = rabbitConfig.HostName,
            Port = rabbitConfig.Port,          // Usually 5671 for TLS
            UserName = rabbitConfig.UserName,
            Password = rabbitConfig.Password,
            VirtualHost = rabbitConfig.VirtualHost,
            Ssl = new SslOption
            {
                Enabled = rabbitConfig.UseSsl,
                ServerName = rabbitConfig.HostName, // Required for proper TLS handshake
                Version = System.Security.Authentication.SslProtocols.Tls12
            }
        });

        services.AddSingleton<RabbitMQResiliency>();
        services.AddSingleton<AsyncRetryPolicy>(sp =>
        {
            var resiliency = sp.GetRequiredService<RabbitMQResiliency>();
            return resiliency.GetRetryPolicy();
        });
        services.AddTransient<IRetryResiliency, RetryResiliency>();
        services.AddSingleton<IRabbitMQConnectionProvider, RabbitMQConnectionProvider>();

        services.AddHostedService<RabbitMQInitializerHostedService>();


        services.AddRabbitMQPublisher();
        services.AddRabbitMQConsumer();


        return builder;
    }


    public static IServiceCollection AddRabbitMQPublisher(this IServiceCollection services)
    {
        services.AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>();
        services.AddHostedService<RabbitMQPublisherHostedService>();

        return services;
    }

    public static IServiceCollection AddRabbitMQConsumer(this IServiceCollection services)
    {
        services.AddScoped<IRabbitAsyncConsumer, RabbitAsyncConsumer>();
        services.AddScoped<IRabbitMqConsumer, RabbitMqConsumer>();
        services.AddHostedService<RabbitMQConsumerHostedService>();

        return services;
    }
}
