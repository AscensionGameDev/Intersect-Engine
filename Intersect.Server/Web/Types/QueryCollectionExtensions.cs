namespace Intersect.Server.Web.Types;

public static class QueryCollectionExtensions
{
    public static int Get(this IQueryCollection query, string key, int defaultValue, int? minValue = null, int? maxValue = null)
    {
        if (!query.TryGetValue(key, out var values))
        {
            return defaultValue;
        }

        var value = values.Select<string?, int?>(value => int.TryParse(value, out var parsed) ? parsed : null)
            .FirstOrDefault(value => value != null) ?? 0;

        if (minValue.HasValue)
        {
            value = Math.Max(minValue.Value, value);
        }

        if (maxValue.HasValue)
        {
            value = Math.Min(maxValue.Value, value);
        }

        return value;
    }
}