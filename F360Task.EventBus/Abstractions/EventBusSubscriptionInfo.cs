namespace F360Task.EventBus.Abstractions;

public class EventBusSubscriptionInfo
{
    public Dictionary<string, Type> EventTypes { get; } = [];

    public JsonSerializerOptions JsonSerializerOptions { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    internal static readonly JsonSerializerOptions DefaultSerializeOptions = new()
    {
        TypeInfoResolver = JsonSerializer.IsReflectionEnabledByDefault
            ? CreateDefaultTypeResolver()
            : JsonTypeInfoResolver.Combine(),
    };

    private static IJsonTypeInfoResolver CreateDefaultTypeResolver()
    => new DefaultJsonTypeInfoResolver();
}
