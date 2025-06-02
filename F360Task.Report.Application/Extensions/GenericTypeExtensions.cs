namespace F360Task.Report.Application.Extensions;

 public static class GenericTypeExtensions
{
    public static string GetGenericTypeName(this Type type)
    {
        string typeName;

        if (type.IsGenericType)
        {
            var genericType = string.Join(",", type.GetGenericArguments().Select(x => x.Name).ToArray());
            typeName = $"{type.Name.Remove(type.Name.IndexOf('`'))}<{genericType}>";
        }
        else
        {
            typeName = type.Name;
        }

        return typeName;
    }

    public static string GetGenericTypeName(this object @object)
    {
        return @object.GetType().GetGenericTypeName();
    }
}