namespace F360Task.Report.Application.Extensions;

public static class ApplicationExtension
{
    public static IHostApplicationBuilder AddAplication(this IHostApplicationBuilder builder)
    {
        
        var services = builder.Services;


        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreateSchedullerReportCommand).Assembly);

            cfg.RegisterServicesFromAssembly(typeof(IdentifiedCommandHandler<,>).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        //Request
        services.AddScoped<IClienteRequestRepository, ClienteRequestRepository>();
        services.AddScoped<IRequestManager, RequestManager>();

        //Validator
        services.AddSingleton<IValidator<CreateSchedullerReportCommand>, CreateSchedullerReportValidator>();

        //Queries
        services.AddScoped<ISchedulerReportQueries, SchedulerReportQueries>();

        return builder;
    }
}
