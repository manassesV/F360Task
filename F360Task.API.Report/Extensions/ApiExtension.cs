namespace F360Task.API.Report.Extensions;

public static class ApiExtension
{
    public static IHostApplicationBuilder AddApi(this IHostApplicationBuilder builder)
    {
        builder.AddAplication();
        builder.AddInfrastructure();
        builder.AddRabbitMqEventBus();

        var services = builder.Services;

        services.AddControllers()
                     .AddJsonOptions(options =>
                     {
                         options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                     });
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        builder.Services.AddCors(options =>
        {

            options.AddPolicy("AllowAll",
                builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
        });

        services.AddAndConfigureSwagger(
            builder.Configuration,
            Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"),
            true);


        services.AddApiVersionConfiguration();


        return builder;
    }
}
