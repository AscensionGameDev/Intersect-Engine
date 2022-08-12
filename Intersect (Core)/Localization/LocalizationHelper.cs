using System.Globalization;

namespace Intersect.Localization;

public static class CultureInfoExtensions
{
    public static bool IsAncestorOf(
        this CultureInfo cultureInfo,
        CultureInfo possibleDescendantCultureInfo
    )
    {
        if (possibleDescendantCultureInfo == CultureInfo.InvariantCulture)
        {
            return true;
        }

        var currentCultureInfo = possibleDescendantCultureInfo.Parent;
        while (currentCultureInfo is not null && currentCultureInfo != CultureInfo.InvariantCulture)
        {
            if (cultureInfo.LCID == currentCultureInfo.LCID)
            {
                return true;
            }

            currentCultureInfo = currentCultureInfo.Parent;
        }

        return false;
    }

    public static bool IsOrIsAncestorOf(
        this CultureInfo cultureInfo,
        CultureInfo possibleDescendantCultureInfo
    ) => cultureInfo.LCID == possibleDescendantCultureInfo.LCID
        || cultureInfo.IsAncestorOf(possibleDescendantCultureInfo);
}

public static class LocalizationHelper
{
    public static readonly CultureInfo CultureEnUS = CultureInfo.GetCultureInfo("en-US");

    public static string? MatchCulture(CultureInfo cultureInfo, IEnumerable<string> strings, char delimiter = '.')
    {
        // Because InvariantCulture is a static readonly we can compare directly
        if (cultureInfo == CultureInfo.InvariantCulture)
        {
            return strings.FirstOrDefault(@string => @string.Count(@char => @char == delimiter) < 2);
        }

        var currentCultureInfo = cultureInfo;
        while (currentCultureInfo is not null && currentCultureInfo != CultureInfo.InvariantCulture)
        {
            var matching = strings.FirstOrDefault(
                @string => @string
                    .Split(delimiter)
                    .Any(
                        segment =>
                            string.Equals(
                                currentCultureInfo.IetfLanguageTag,
                                segment,
                                StringComparison.OrdinalIgnoreCase
                            )
                    )
            );

            if (matching != default)
            {
                return matching;
            }

            currentCultureInfo = currentCultureInfo.Parent;
        }

        if (!CultureEnUS.IsOrIsAncestorOf(cultureInfo))
        {
            var matching = MatchCulture(CultureEnUS, strings, delimiter);

            if (matching != default)
            {
                return matching;
            }
        }

        return MatchCulture(CultureInfo.InvariantCulture, strings, delimiter);
    }
}
