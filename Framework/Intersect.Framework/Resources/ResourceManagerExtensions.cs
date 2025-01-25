using System.Globalization;
using System.Resources;

namespace Intersect.Framework.Resources;

public static class ResourceManagerExtensions
{
    public static string? GetStringWithFallback(
        this ResourceManager resourceManager,
        string name,
        CultureInfo? cultureInfo = null,
        bool fallbackToResourceName = false
    )
    {
        cultureInfo ??= CultureInfo.CurrentCulture;

        while (cultureInfo.LCID != CultureInfo.InvariantCulture.LCID)
        {
            var value = resourceManager.GetString(name, cultureInfo);
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            cultureInfo = cultureInfo.Parent;
        }

        return fallbackToResourceName ? name : null;
    }
}