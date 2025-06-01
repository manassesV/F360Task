using Microsoft.AspNetCore.Mvc.Versioning;

namespace F360Task.API.ApiVersionSupport;

public static class ApiVersionConfiguration
{
    public static IServiceCollection AddApiVersionConfiguration(this IServiceCollection services,
        ApiVersion defaultVersion = null)
    {
        defaultVersion ??= ApiVersion.Default;
        services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV"; // Isso gera "v1", "v1.0", etc.
        });
        services.AddApiVersioning(options =>
        {
            //Set Default version
            options.DefaultApiVersion = defaultVersion;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.UseApiBehavior = true;
            options.ReportApiVersions = true;

            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new QueryStringApiVersionReader(),
                new QueryStringApiVersionReader("v"),
                new HeaderApiVersionReader("api-version"),
                new HeaderApiVersionReader("v"),
                new MediaTypeApiVersionReader(),
                new MediaTypeApiVersionReader("api-version"));
        });

        return services;
    }
}