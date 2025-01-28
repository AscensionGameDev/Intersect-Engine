using System.Text.RegularExpressions;

namespace Intersect.Localization;

[Serializable]
public partial class LocalizedString(string value) : Localized
{
    private readonly string _value = value;

    public int ArgumentCount { get; } = CountArguments(value);

    public static implicit operator LocalizedString(string value) => new(value);

    public static implicit operator string(LocalizedString localizedString) => localizedString._value;

    public override string ToString() => _value;

    public string ToString(params object[] args)
    {
        try
        {
            return args?.Length == 0 ? _value : string.Format(_value, args ?? []);
        }
        catch (FormatException)
        {
            return "Format Exception!";
        }
    }

    private static readonly Regex PatternArgument = new(
        "\\{(?<argumentIndex>\\d+)\\}",
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Multiline | RegexOptions.NonBacktracking
    );

    public static int CountArguments(string formatString)
    {
        HashSet<int> argumentIndices = [];

        var matches = PatternArgument.Matches(formatString);
        foreach (Match match in matches)
        {
            if (!match.Success)
            {
                continue;
            }

            var rawArgumentIndex = match.Groups["argumentIndex"].Value;
            if (!int.TryParse(rawArgumentIndex, out var argumentIndex))
            {
                continue;
            }

            _ = argumentIndices.Add(argumentIndex);
        }

        return argumentIndices.Count;
    }
}
