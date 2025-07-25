﻿namespace F360Task.API.Report.Swagger;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;
    private readonly SwaggerApplicationSettings _settings;

    public ConfigureSwaggerOptions(
        IApiVersionDescriptionProvider provider,
        IOptionsMonitor<SwaggerApplicationSettings> settingsMonitor
        )
    {
        _provider = provider;
        _settings = settingsMonitor.CurrentValue;
    }

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName,
                CreateInfoForApiVersion(description, _settings));
        }
    }

    internal OpenApiInfo CreateInfoForApiVersion(
        ApiVersionDescription description,
        SwaggerApplicationSettings settings)
    {
        var versionDesc = settings.Descriptions
            .FirstOrDefault(x =>
            x.MajorVersion == (description.ApiVersion.MajorVersion??0)
            && x.MinorVersion == (description.ApiVersion.MinorVersion ?? 0)
            && (string.IsNullOrEmpty(description.ApiVersion.Status)
            || x.Status == description.ApiVersion.Status));

        var info = new OpenApiInfo()
        {
            Title = settings.Title,
            Version = description.ApiVersion.ToString(),
            Description = versionDesc?.Description,
            Contact = new OpenApiContact()
            {
                Name = settings.ContactName,
                Email = settings.ContactEmail
            },
            License = new OpenApiLicense()
            {
                Name = "Use under LICX",
                Url = new Uri("https://example.com/license")
            }
        };

        if (description.IsDeprecated)
        {
            info.Description += "<p><font color='red'>This API version has been deprecated.</font></p>";
        }

        return info;
    }
}
