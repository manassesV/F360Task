namespace F360Task.API.Report.Swagger.Models;

public class SwaggerApplicationSettings
{
    public string Title { get; set; } = string.Empty;
    public List<SwaggerVersionDescription> Descriptions { get; set; } = new();
    public string ContactName { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
}
