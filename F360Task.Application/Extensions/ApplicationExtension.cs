using F360Task.Application.Validations;

namespace F360Task.Application.Extensions;

public static class ApplicationExtension
{
    public static IHostApplicationBuilder AddAplication(this IHostApplicationBuilder builder)
    {
        
        var services = builder.Services;


        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreateSchedullerEmailCommand).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(CreateSchedullerReportCommand).Assembly);

            cfg.RegisterServicesFromAssembly(typeof(IdentifiedCommandHandler<,>).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        //Request
        services.AddScoped<IClienteRequestRepository, ClienteRequestRepository>();
        services.AddScoped<IRequestManager, RequestManager>();

        //Validator
        services.AddSingleton<IValidator<CreateSchedullerEmailCommand>, CreateSchedulerEmailValidator>();
        services.AddSingleton<IValidator<CreateSchedullerReportCommand>, CreateSchedullerReportValidator>();

        //Queries
        services.AddScoped<ISchedulerEmailQueries, SchedulerEmailQueries>();
        services.AddScoped<ISchedulerReportQueries, SchedulerReportQueries>();

        return builder;
    }
}
