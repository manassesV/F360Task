namespace F360Task.Application.Extensions;

public static class ApplicationExtension
{
    public static IHostApplicationBuilder AddAplication(this IHostApplicationBuilder builder)
    {
        
        var service = builder.Services;


        service.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<CreateSchedullerEmailCommand>();
            cfg.RegisterServicesFromAssemblyContaining<CreateSchedullerReportCommand>();
        });


        // Se estiver usando pipeline behavior de validação
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        //Queries
        service.AddScoped<ISchedulerEmailQueries, SchedulerEmailQueries>();
        service.AddScoped<ISchedulerReportQueries, SchedulerReportQueries>();

        return builder;
    }
}
