using Intersect.Core;
using Intersect.Framework.Reflection;
using Microsoft.Extensions.Logging;

namespace Intersect.Client.Framework.GenericClasses;

public static class KeysExtensions
{
    public static string GetName(this Keys key, bool isModifier = false)
    {
        var name = Enum.GetName(key);
        if (!string.IsNullOrWhiteSpace(name))
        {
            return name;
        }

        ApplicationContext.CurrentContext.Logger.LogWarning(
            "Invalid {KeyType} {Modifier}, no name found in {TypeName}",
            isModifier ? "modifier key" : "key",
            key,
            typeof(Keys).GetName(qualified: true)
        );

        return "Unknown Key";
    }
}